using Dangl.Data.Shared;
using Dangl.Identity.Client.Mvc.Services;
using Dangl.OpenCDE.Core.Extensions;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Data.Models;
using Dangl.OpenCDE.Data.Repository;
using Dangl.OpenCDE.Shared.Models.Controllers.OpenCdeIntegration;
using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models;
using Dangl.OpenCDE.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Dangl.OpenCDE.Core.Controllers.CdeApi
{
    [Route("api/open-cde-integration/upload")]
    public class OpenCdeUploadIntegrationController : CdeAppControllerBase
    {
        private readonly SignInManager<CdeUser> _signInManager;
        private readonly UserManager<CdeUser> _userManager;
        private readonly IOpenCdeDocumentSelectionRepository _openCdeDocumentSelectionService;
        private readonly IDocumentsRepository _documentsRepository;
        private readonly IUserInfoService _userInfoService;

        public OpenCdeUploadIntegrationController(SignInManager<CdeUser> signInManager,
            UserManager<CdeUser> userManager,
            IOpenCdeDocumentSelectionRepository openCdeDocumentSelectionService,
            IDocumentsRepository documentsRepository,
            IUserInfoService userInfoService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _openCdeDocumentSelectionService = openCdeDocumentSelectionService;
            _documentsRepository = documentsRepository;
            _userInfoService = userInfoService;
        }

        [AllowAnonymous]
        [HttpGet("sessions/{documentSessionId}/simple-auth")]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(SimpleAuthToken), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUploadSessionSimpleAuthDataAsync(Guid documentSessionId)
        {
            var userId = await _openCdeDocumentSelectionService.GetUserSessionDataForDocumentUploadAsync(documentSessionId);
            if (!userId.IsSuccess)
            {
                return BadRequest(new ApiError(userId.ErrorMessage));
            }

            return Ok(new SimpleAuthToken
            {
                Jwt = userId.Value.tokenStorage.JsonWebToken,
                ExpiresAt = userId.Value.tokenStorage.ExpiresAt
            });
        }

        [HttpPost("sessions/{documentSessionId}/project")]
        [ProducesResponseType(typeof(UploadSessionProjectAssignmentResultGet), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetProjectForUploadSession(Guid documentSessionId, [FromBody] UploadSessionProjectAssignmentPost projectSelection)
        {
            var repoResult = await _openCdeDocumentSelectionService
                    .SelectProjectForDocumentUploadSession(projectSelection.ProjectId, documentSessionId);
            if (!repoResult.IsSuccess)
            {
                return BadRequest(new ApiError(repoResult.ErrorMessage));
            }

            var selectedDocumentsUrl = Url.Action(nameof(GetUploadFileDetailsAsync).WithoutAsyncSuffix(),
                nameof(OpenCdeUploadIntegrationController).WithoutControllerSuffix(), new
                {
                    documentSessionId = documentSessionId
                }, Request.IsHttps ? "https" : "http", Request.Host.ToString());

            var url = new Uri(repoResult.Value);
            var query = HttpUtility.ParseQueryString(url.Query);
            query.Add("upload_documents_url", selectedDocumentsUrl);
            var uriBuilder = new UriBuilder(url);
            uriBuilder.Query = query.ToString();
            var callbackUrl = uriBuilder.ToString();

            return Ok(new UploadSessionProjectAssignmentResultGet
            {
                ClientCallbackUrl = callbackUrl
            });
        }

        [HttpPost("sessions/{documentSessionId}/upload-instructions")]
        [ProducesResponseType(typeof(DocumentsToUpload), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUploadFileDetailsAsync(Guid documentSessionId,
            [FromBody] UploadFileDetails uploadFileDetails)
        {
            var uploadInstructions = await _openCdeDocumentSelectionService.GetUploadInstructionsAsync(documentSessionId, uploadFileDetails);
            if (!uploadInstructions.IsSuccess)
            {
                return BadRequest(new ApiError(uploadInstructions.ErrorMessage));
            }

            foreach (var fileInstruction in uploadInstructions.Value._DocumentsToUpload)
            {
                fileInstruction.UploadCompletion = new LinkData
                {
                    Url = Url.Action(nameof(OpenCdeUploadIntegrationController.MarkFileUploadAsCompletedAsync).WithoutAsyncSuffix(),
                        nameof(OpenCdeUploadIntegrationController).WithoutControllerSuffix(), new
                        {
                            documentSessionId = documentSessionId,
                            sessionFileId = fileInstruction.SessionFileId
                        }, Request.IsHttps ? "https" : "http", Request.Host.ToString())
                };
                fileInstruction.UploadCancellation = new LinkData
                {
                    Url = Url.Action(nameof(OpenCdeUploadIntegrationController.MarkFileUploadAsCancelledAsync).WithoutAsyncSuffix(),
                        nameof(OpenCdeUploadIntegrationController).WithoutControllerSuffix(), new
                        {
                            documentSessionId = documentSessionId,
                            sessionFileId = fileInstruction.SessionFileId
                        }, Request.IsHttps ? "https" : "http", Request.Host.ToString())
                };
            }

            return Ok(uploadInstructions.Value);
        }

        [HttpPost("sessions/{documentSessionId}/upload-completion/{sessionFileId}")]
        [ProducesResponseType(typeof(DocumentVersion), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> MarkFileUploadAsCompletedAsync(Guid documentSessionId, string sessionFileId)
        {
            var result = await _openCdeDocumentSelectionService
                    .MarkCdeSessionFileUploadAsFinishedAsync(documentSessionId, sessionFileId);
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiError(result.ErrorMessage));
            }
            var documentVersion = result.Value.ToDocumentVersionForDocument(Url, Request.IsHttps, Request.Host.ToString());
            return Ok(documentVersion);
        }

        [HttpPost("sessions/{documentSessionId}/upload-cancellation/{sessionFileId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> MarkFileUploadAsCancelledAsync(Guid documentSessionId, string sessionFileId)
        {
            var result = await _openCdeDocumentSelectionService
                    .MarkCdeSessionFileUploadAsCancelledAsync(documentSessionId, sessionFileId);
            if (!result.IsSuccess)
            {
                return BadRequest(new ApiError(result.ErrorMessage));
            }

            return NoContent();
        }
    }
}
