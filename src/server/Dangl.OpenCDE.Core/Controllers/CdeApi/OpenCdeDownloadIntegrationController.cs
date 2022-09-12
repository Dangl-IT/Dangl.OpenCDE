using Dangl.Data.Shared;
using Dangl.Identity.Client.Mvc.Services;
using Dangl.OpenCDE.Core.Utilities;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Data.Models;
using Dangl.OpenCDE.Data.Repository;
using Dangl.OpenCDE.Shared.Models.Controllers.OpenCdeIntegration;
using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Dangl.OpenCDE.Core.Controllers.CdeApi
{
    [Route("api/open-cde-integration")]
    public class OpenCdeDownloadIntegrationController : CdeAppControllerBase
    {
        private readonly SignInManager<CdeUser> _signInManager;
        private readonly UserManager<CdeUser> _userManager;
        private readonly IOpenCdeDocumentSelectionRepository _openCdeDocumentSelectionService;
        private readonly IDocumentsRepository _documentsRepository;
        private readonly IUserInfoService _userInfoService;

        public OpenCdeDownloadIntegrationController(SignInManager<CdeUser> signInManager,
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
        public async Task<IActionResult> GetSessionSimpleAuthDataAsync(Guid documentSessionId)
        {
            // TODO CHECK IF THIS ACTION IS STILL REQUIRED
            var userId = await _openCdeDocumentSelectionService.GetUserSessionDataForDocumentSessionAsync(documentSessionId);
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

        [HttpPost("sessions/{documentSessionId}/document-selection")]
        [ProducesResponseType(typeof(DocumentSelectionGet), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetDocumentSelection(Guid documentSessionId, [FromBody] DocumentSelectionPost documentSelection)
        {
            var repoResult = await _openCdeDocumentSelectionService
                .FinalizeOpenCdeDocumentSelectionAsync(documentSessionId, documentSelection.DocumentId);
            if (!repoResult.IsSuccess)
            {
                return BadRequest(new ApiError(repoResult.ErrorMessage));
            }

            var selectedDocumentsUrl = Url.Action(nameof(GetDocumentSelectionDataAsync).WithoutAsyncSuffix(),
                nameof(OpenCdeDownloadIntegrationController).WithoutControllerSuffix(), new
                {
                    documentSelectionId = repoResult.Value.SelectionId
                }, Request.IsHttps ? "https" : "http", Request.Host.ToString());

            var url = new Uri(repoResult.Value.ClientCallbackUrl);
            var query = HttpUtility.ParseQueryString(url.Query);
            query.Add("selected_documents_url", selectedDocumentsUrl);
            query.Add("selection_context", repoResult.Value.SelectionId.ToString());
            var uriBuilder = new UriBuilder(url);
            uriBuilder.Query = query.ToString();
            var callbackUrl = uriBuilder.ToString();

            return Ok(new DocumentSelectionGet
            {
                CallbackUrl = callbackUrl
            });
        }

        [HttpGet("documents/{documentId}/metadata")]
        [ProducesResponseType(typeof(DocumentMetadata), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetDocumentMetadataAsync(Guid documentId)
        {
            var document = await _documentsRepository
                .GetDocumentByIdAsync(documentId);

            if (!document.IsSuccess)
            {
                return BadRequest(new ApiError(document.ErrorMessage));
            }

            var metadata = new DocumentMetadata
            {
                Metadata = new List<DocumentMetadataEntry>
                {
                    new DocumentMetadataEntry
                    {
                        Name = nameof(document.Value.Name),
                        Value = new List<string> { document.Value.Name },
                        DataType = DocumentMetadataEntry.DataTypeEnum.StringEnum
                    },
                    new DocumentMetadataEntry
                    {
                        Name = nameof(document.Value.FileName),
                        Value = new List<string> { document.Value.FileName },
                        DataType = DocumentMetadataEntry.DataTypeEnum.StringEnum
                    },
                    new DocumentMetadataEntry
                    {
                        Name = nameof(document.Value.CreatedAtUtc),
                        Value = new List<string> { document.Value.CreatedAtUtc.ToString("O") },
                        DataType = DocumentMetadataEntry.DataTypeEnum.DateTimeEnum
                    },
                    new DocumentMetadataEntry
                    {
                        Name = nameof(document.Value.FileSizeInBytes),
                        Value = new List<string> { document.Value.FileSizeInBytes.ToString() },
                        DataType = DocumentMetadataEntry.DataTypeEnum.Integer64Enum
                    }
                }
            };

            return Ok(metadata);
        }

        [HttpGet("documents/{documentId}/versions")]
        [ProducesResponseType(typeof(DocumentVersions), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetDocumentVersionsAsync(Guid documentId)
        {
            var document = await _documentsRepository
                .GetDocumentByIdAsync(documentId);

            if (!document.IsSuccess)
            {
                return BadRequest(new ApiError(document.ErrorMessage));
            }

            var documentVersion = GetDocumentVersionForDocument(document.Value);

            var documentVersions = new DocumentVersions
            {
                Documents = new List<DocumentVersion> { documentVersion }
            };

            return Ok(documentVersions);
        }

        [HttpGet("documents/{documentId}/reference")]
        [ProducesResponseType(typeof(SelectedDocuments), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetDocumentReferenceAsync(Guid documentId)
        {
            var document = await _documentsRepository
                .GetDocumentByIdAsync(documentId);

            if (!document.IsSuccess)
            {
                return BadRequest(new ApiError(document.ErrorMessage));
            }

            var documentVersion = GetDocumentVersionForDocument(document.Value);
            var selection = new SelectedDocuments
            {
                Documents = new List<DocumentVersion> { documentVersion }
            };

            return Ok(selection);
        }

        [HttpGet("document-selections/{documentSelectionId}")]
        [ProducesResponseType(typeof(DocumentVersion), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetDocumentSelectionDataAsync(Guid documentSelectionId)
        {
            var document = await _openCdeDocumentSelectionService.GetDocumentForSelectionAsync(documentSelectionId);
            if (!document.IsSuccess)
            {
                return BadRequest(new ApiError(document.ErrorMessage));
            }

            var documentVersion = GetDocumentVersionForDocument(document.Value);
            var selection = new SelectedDocuments
            {
                Documents = new List<DocumentVersion> { documentVersion }
            };

            return Ok(selection);
        }

        private DocumentVersion GetDocumentVersionForDocument(DocumentDto document)
        {
            return new DocumentVersion
            {
                Title = document.Name,
                VersionNumber = "1",
                CreationDate = document.CreatedAtUtc,
                FileDescription = new FileDescription
                {
                    Name = document.FileName,
                    SizeInBytes = document.FileSizeInBytes ?? 0
                },
                Links = new DocumentVersionLinks
                {
                    DocumentVersionDownload = new LinkData
                    {
                        Url = GetAbsoluteBaseUrl(nameof(DocumentsController), nameof(DocumentsController.DownloadDocumentAsync), new
                        {
                            projectId = document.ProjectId,
                            documentId = document.Id
                        })
                    },
                    DocumentVersionMetadata = new LinkData
                    {
                        Url = GetAbsoluteBaseUrl(nameof(OpenCdeDownloadIntegrationController), nameof(GetDocumentMetadataAsync), new
                        {
                            documentId = document.Id
                        })
                    },
                    DocumentVersion = new LinkData
                    {
                        Url = GetAbsoluteBaseUrl(nameof(OpenCdeDownloadIntegrationController), nameof(GetDocumentReferenceAsync), new
                        {
                            documentId = document.Id
                        })
                    },
                    DocumentVersions = new LinkData
                    {
                        Url = GetAbsoluteBaseUrl(nameof(OpenCdeDownloadIntegrationController), nameof(GetDocumentVersionsAsync), new
                        {
                            documentId = document.Id
                        })
                    }
                }
            };
        }

        private string GetAbsoluteBaseUrl(string controllerName, string actionName, object routeParameters)
        {
            controllerName = controllerName.WithoutControllerSuffix();
            actionName = actionName.WithoutAsyncSuffix();
            var protocol = Request.IsHttps ? "https" : "http";

            var url = Url.Action(actionName, controllerName, routeParameters, protocol, Request.Host.ToString(), null);
            return url;
        }
    }
}
