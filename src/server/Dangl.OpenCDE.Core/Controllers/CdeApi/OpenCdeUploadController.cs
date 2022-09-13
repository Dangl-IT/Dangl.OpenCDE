using Dangl.Data.Shared;
using Dangl.OpenCDE.Core.Configuration;
using Dangl.OpenCDE.Data.Repository;
using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Core.Controllers.CdeApi
{
    [Route("api/opencde/1.0")]
    public class OpenCdeUploadController : CdeAppControllerBase
    {
        private readonly IOpenCdeDocumentSelectionRepository _openCdeDocumentSelectionService;
        private readonly OpenCdeSettings _openCdeSettings;

        public OpenCdeUploadController(IOpenCdeDocumentSelectionRepository openCdeDocumentSelectionService,
            OpenCdeSettings openCdeSettings)
        {
            _openCdeDocumentSelectionService = openCdeDocumentSelectionService;
            _openCdeSettings = openCdeSettings;
        }
        
        [HttpPost("upload-documents")]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(DocumentUploadSessionInitialization), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDocumentDiscoveryData([FromBody, Required] UploadDocuments documentUploadData)
        {
            if (!(documentUploadData?.Files?.Any() ?? false))
            {
                return BadRequest(new ApiError("No files were provided"));
            }

            var userJwtExpiresAtClaim = User
                .Claims
                .First(uc => uc.Type == "exp");
            var userJwtExpiresAt = long.Parse(userJwtExpiresAtClaim.Value);
            var userJwt = HttpContext
                .Request
                .Headers["Authorization"]
                .First()
                .Substring("Bearer ".Length);

            var sessionInitializationResult = await _openCdeDocumentSelectionService
                .PrepareOpenCdeDocumentUploadSelectionAsync(documentUploadData.Callback?.Url,
                userJwt,
                userJwtExpiresAt,
                documentUploadData.Files);
            if (!sessionInitializationResult.IsSuccess)
            {
                return BadRequest(new ApiError(sessionInitializationResult.ErrorMessage));
            }

            var uploadUiUrl = $"{_openCdeSettings.AppBaseUrl.TrimEnd('/')}/opencde-upload-documents?documentSessionId={sessionInitializationResult.Value.sessionId}";

            var callbackResult = new DocumentUploadSessionInitialization
            {
                MaxSizeInBytes = OpenCdeDocumentSelectionRepository.FILE_UPLOAD_MAX_SIZE_IN_BYTES,
                UploadUiUrl = uploadUiUrl,
                ExpiresIn = sessionInitializationResult.Value.validForSeconds
            };
            return Ok(callbackResult);
        }
    }
}
