using Dangl.OpenCDE.Client.Hubs;
using Dangl.OpenCDE.Client.Models;
using Dangl.OpenCDE.Client.Services;
using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models;
using Dangl.OpenCDE.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dangl.OpenCDE.Client.Controllers
{
    [Route("cde-server-callback")]
    public class CdeServerCallbackController : Controller
    {
        private readonly ClientNotificationsService _clientNotificationsService;
        private readonly IHubContext<CdeClientHub> _hubContext;
        private readonly OpenCdeUploadOperationsCache _openCdeUploadOperationsCache;

        public CdeServerCallbackController(ClientNotificationsService clientNotificationsService,
            IHubContext<CdeClientHub> hubContext,
            OpenCdeUploadOperationsCache openCdeUploadOperationsCache)
        {
            _clientNotificationsService = clientNotificationsService;
            _hubContext = hubContext;
            _openCdeUploadOperationsCache = openCdeUploadOperationsCache;
        }

        [HttpGet("upload")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> HandleCdeUploadCallbackAsync([FromQuery] string state,
            [FromQuery(Name = "upload_documents_url")] string uploadDocumentsUrl,
            [FromQuery(Name = "user_cancelled_selection")] bool cancelledByUser)
        {
            if (!_openCdeUploadOperationsCache.UploadTasksByState.TryGetValue(state, out var cachedData))
            {
                await _clientNotificationsService.SendErrorToClientAsync("There is no local operation saved for the received request.");
            }
            else if (cancelledByUser)
            {
                await _clientNotificationsService.SendInformationToClientAsync("The upload operation was cancelled by the CDE.");
            }
            else if (string.IsNullOrWhiteSpace(uploadDocumentsUrl))
            {
                await _clientNotificationsService.SendErrorToClientAsync("The CDE did not return the upload instructions in the proper format.");
            }
            else
            {
                await _clientNotificationsService.SendInformationToClientAsync("Received CDE upload instructions, preparing to upload files.");
                
                using var httpClient = new HttpClient();

                var uploadFileDetails = new UploadFileDetails
                {
                    Files = cachedData
                        .Files
                        .Select(f => new UploadFileDetail
                        {
                            SessionFileId = f.SessionFileId,
                            SizeInBytes = f.FileSizeInBytes
                        })
                        .ToList()
                };

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cachedData.AccessToken);

                var jsonPostRequet = HttpRequestHelper.GetJsonPostRequest(uploadFileDetails, uploadDocumentsUrl);

                try
                {
                    var cdeResponse = await httpClient.SendAsync(jsonPostRequet);
                    if (!cdeResponse.IsSuccessStatusCode)
                    {
                        await _clientNotificationsService.SendErrorToClientAsync("Failed to get upload information from the CDE, the response status code was: "
                            + cdeResponse.StatusCode
                            + " (" + (int)cdeResponse.StatusCode + ")");
                    }
                    else
                    {
                        var responseJson = await cdeResponse.Content.ReadAsStringAsync();
                        var cdeUploadInstructions = JsonConvert.DeserializeObject<DocumentsToUpload>(responseJson);
                        _openCdeUploadOperationsCache.UploadTasks.Enqueue(new OpenCdeUploadTask
                        {
                            CdeUploadInstructions = cdeUploadInstructions,
                            ClientFileData = cachedData
                        });
                    }
                }
                catch
                {
                    await _clientNotificationsService.SendErrorToClientAsync("The upload instructions received by the CDE could not be correctly validated.");
                }
            }

            var siteContent = @"<p>Thank you! The upload preparationis finished, please close this browser tab and switch back to the OpenCDE Client.</p>";

            var content = HtmlTemplateProvider.GetHtmlContent(string.Empty, siteContent, "OpenCDE Upload Preparation");

            return Content(content, "text/html");
        }

        [HttpGet("download")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> HandleCdeDownloadCallbackAsync([FromQuery] string state,
            [FromQuery(Name = "selected_documents_url")] string selectedDocumentsUrl,
            [FromQuery(Name = "selected_url")] string selectedUrl)
        {
            selectedDocumentsUrl ??= selectedUrl;
            selectedDocumentsUrl ??= TryGetSelectedDocumentsUrlFromQueryParametersOrNull();

            await _hubContext
                .Clients
                .All
                .SendAsync("DocumentSelectionResultCallback", new
                {
                    selectedDocumentsUrl,
                    state
                });

            var siteContent = @"<p>Thank you! The document selection is finished, please close this browser tab and switch back to the OpenCDE Client.</p>";

            var content = HtmlTemplateProvider.GetHtmlContent(string.Empty, siteContent, "OpenCDE Document Selection");

            return Content(content, "text/html");
        }

        private string TryGetSelectedDocumentsUrlFromQueryParametersOrNull()
        {
            // This was discovered where a CDE returned a link like this:
            // https://localhost?state=123?selected_url=https://example.com
            // This had three issues:
            // 1. The 'state' parameter was preserved from the client, but then the second query parameter was not separated by an
            //    ampersand '&' but again by a question mark '?'
            // 2. The second parameter was actually called 'selected_url', but it should have been 'selected_documents_url'
            // 3. The second parameter was not URL encoded, but it luckily does still work that way😀
            if (HttpContext.Request.Query.Count == 1
                && HttpContext.Request.Query.ContainsKey("state"))
            {
                var queryParam = HttpContext.Request.Query["state"].Single();

                var cleanedQuery = queryParam
                    .SkipWhile(c => c != '?')
                    .Skip(1)
                    .Select(c => c.ToString())
                    .Aggregate((c, n) => c + n);

                if (cleanedQuery.StartsWith("selected_url=", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return cleanedQuery[13..]; // "selected_url=".Length = 13
                }
                else if (cleanedQuery.StartsWith("selected_documents_url=", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return cleanedQuery[23..]; // "selected_documents_url=".Length = 23
                }
            }

            return null;
        }
    }
}
