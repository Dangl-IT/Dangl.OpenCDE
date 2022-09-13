using Dangl.OpenCDE.Client.Models;
using Dangl.OpenCDE.Client.Services;
using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models;
using Dangl.OpenCDE.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Client.Controllers
{
    [Route("documents-upload-handler")]
    public class DocumentsUploadHandlerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OpenCdeUploadOperationsCache _openCdeUploadOperationsCache;

        public DocumentsUploadHandlerController(IHttpClientFactory httpClientFactory,
            OpenCdeUploadOperationsCache openCdeUploadOperationsCache)
        {
            _httpClientFactory = httpClientFactory;
            _openCdeUploadOperationsCache = openCdeUploadOperationsCache;
        }

        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [HttpPost("start-upload")]
        public async Task<IActionResult> PrepareDocumentUploadAndOpenSystemBrowserAsync([FromBody] DocumentUploadInitializationParameters parameters)
        {
            if (parameters == null)
            {
                return BadRequest();
            }
            
            if (!(parameters.Files?.Any() ?? false))
            {
                return BadRequest();
            }

            var callbackUrl = Url.Action(nameof(CdeServerCallbackController.HandleCdeUploadCallbackAsync).WithoutAsyncSuffix(),
                nameof(CdeServerCallbackController).WithoutControllerSuffix(), new
            {
                state = parameters.ClientState
            }, Request.IsHttps ? "https" : "http", Request.Host.ToString(), null);

            var uploadDocumentsUrl = parameters.OpenCdeBaseUrl
                .TrimEnd('/')
                + "/upload-documents";

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", parameters.AccessToken);

            var callbackParameters = JObject.FromObject(new UploadDocuments
            {
                Callback = new CallbackLink
                {
                    ExpiresIn = 3600,
                    Url = callbackUrl
                },
                Files = parameters
                    .Files
                    .Select(f => new FileToUpload
                    {
                        FileName = f.FileName,
                        SessionFileId = f.SessionFileId
                    })
                    .ToList()
            });
            

            var initialRequest = HttpRequestHelper.GetJsonPostRequest(callbackParameters, uploadDocumentsUrl);
            var response = await httpClient.SendAsync(initialRequest);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<DocumentUploadSessionInitialization>(responseContent);

                // This is a workaround, since it's required at the moment by some vendors
                var documentSelectionUrl = data.UploadUiUrl += $"&access_token={parameters.AccessToken}";

                _openCdeUploadOperationsCache.UploadTasksByState.Add(parameters.ClientState, parameters);

                SystemBrowserService.OpenSystemBrowser(documentSelectionUrl);
                return NoContent();
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return BadRequest(responseContent);
            }
        }
    }
}
