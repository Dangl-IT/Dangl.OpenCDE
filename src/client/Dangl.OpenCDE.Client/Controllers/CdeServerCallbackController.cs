using Dangl.OpenCDE.Client.Hubs;
using Dangl.OpenCDE.Client.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Threading.Tasks;

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
            [FromQuery(Name = "selected_documents_url")] string selectedDocumentsUrl)
        {
            await _hubContext
                .Clients
                .All
                .SendAsync("DocumentSelectionResultCallback", selectedDocumentsUrl);

            var siteContent = @"<p>Thank you! The document selection is finished, please close this browser tab and switch back to the OpenCDE Client.</p>";

            var content = HtmlTemplateProvider.GetHtmlContent(string.Empty, siteContent, "OpenCDE Document Selection");

            return Content(content, "text/html");
        }
    }
}
