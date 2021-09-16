using Dangl.Data.Shared;
using Dangl.OpenCDE.Core.Configuration;
using Dangl.OpenCDE.Data.Repository;
using Dangl.OpenCDE.Shared.Models.CdeApi;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Core.Controllers.CdeApi
{
    [Route("api/opencde/1.0")]
    public class DocumentSelectionController : CdeAppControllerBase
    {
        private readonly IOpenCdeDocumentSelectionRepository _openCdeDocumentSelectionService;
        private readonly OpenCdeSettings _openCdeSettings;

        public DocumentSelectionController(IOpenCdeDocumentSelectionRepository openCdeDocumentSelectionService,
            OpenCdeSettings openCdeSettings)
        {
            _openCdeDocumentSelectionService = openCdeDocumentSelectionService;
            _openCdeSettings = openCdeSettings;
        }

        [HttpPost("select-documents")]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(DocumentDiscoverySessionInitialization), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDocumentDiscoveryData(DocumentDiscoveryPost discoveryMetadata)
        {
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
                .PrepareOpenCdeDocumentSelectionAsync(discoveryMetadata?.CallbackLink.Url,
                userJwt,
                userJwtExpiresAt);
            if (!sessionInitializationResult.IsSuccess)
            {
                return BadRequest(new ApiError(sessionInitializationResult.ErrorMessage));
            }

            var selectDocumentsUrl = $"{_openCdeSettings.AppBaseUrl.TrimEnd('/')}/opencde-select-documents?documentSessionId={sessionInitializationResult.Value.sessionId}";

            var callbackResult = new DocumentDiscoverySessionInitialization
            {
                SelectDocumentsUrl = selectDocumentsUrl,
                ExpiresIn = sessionInitializationResult.Value.validForSeconds
            };
            return Ok(callbackResult);
        }
    }
}
