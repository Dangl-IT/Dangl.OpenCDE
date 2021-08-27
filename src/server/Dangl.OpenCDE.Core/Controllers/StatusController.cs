using Dangl.OpenCDE.Shared;
using Dangl.OpenCDE.Shared.Models.Controllers.Status;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Dangl.OpenCDE.Core.Controllers
{
    /// <summary>
    /// This controller reports the health status of the Dangl.OpenCDE API
    /// </summary>
    [Route("api/status")]
    [AllowAnonymous]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public StatusController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        /// <summary>
        /// Reports the health status of the Dangl.OpenCDE API
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(StatusGet), 200)]
        public IActionResult GetStatus()
        {
            var status = new StatusGet
            {
                IsHealthy = true,
                Version = VersionsService.Version,
                InformationalVersion = VersionsService.InformationalVersion,
                Environment = _environment.EnvironmentName
            };
            return Ok(status);
        }
    }
}
