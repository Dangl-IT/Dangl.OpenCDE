using Dangl.OpenCDE.Client.Models;
using Dangl.OpenCDE.Client.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Dangl.OpenCDE.Client.Controllers
{
    [Route("documents-selection-handler")]
    public class DocumentsSelectionHandlerController : Controller
    {
        [ProducesResponseType(typeof(DocumentSelectionCallbackParameters), (int)HttpStatusCode.OK)]
        public IActionResult GetDocumentSelectionCallbackUrl([FromQuery] string clientState)
        {
            var callbackUrl = Url.Action("HandleCdeCallback", "CdeServerCallback", new
            {
                state = clientState
            }, Request.IsHttps ? "https" : "http", Request.Host.ToString(), null);

            return Ok(new DocumentSelectionCallbackParameters
            {
                CallbackUrl = callbackUrl
            });
        }

        [HttpPost("")]
        public IActionResult OpenCdeDocumentSelectionPage(SystemBrowserUrlOpenCommand page)
        {
            SystemBrowserService.OpenSystemBrowser(page.Url);
            return NoContent();
        }
    }
}
