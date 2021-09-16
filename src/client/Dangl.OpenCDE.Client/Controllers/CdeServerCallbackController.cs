using Dangl.OpenCDE.Client.Hubs;
using Dangl.OpenCDE.Client.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Dangl.OpenCDE.Client.Controllers
{
    [Route("cde-server-callback")]
    public class CdeServerCallbackController : Controller
    {
        private readonly IHubContext<CdeClientHub> _hubContext;

        public CdeServerCallbackController(IHubContext<CdeClientHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> HandleCdeCallbackAsync([FromQuery] string state,
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
