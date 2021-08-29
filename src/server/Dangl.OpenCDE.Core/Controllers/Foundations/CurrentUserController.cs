using Dangl.Data.Shared;
using Dangl.Identity.Client.Mvc.Services;
using Dangl.OpenCDE.Shared.Models.Foundations;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Core.Controllers.Foundations
{
    [Route("foundation/1.0/current-user")]
    public class CurrentUserController : CdeAppControllerBase
    {
        private readonly IUserInfoService _userInfoService;

        public CurrentUserController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(UserGet), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiBehaviorOptions), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetCurrentUserDataAsync()
        {
            var userIsAuthenticated = await _userInfoService.UserIsAuthenticatedAsync();
            if (!userIsAuthenticated)
            {
                return BadRequest(new ApiError("No user is authenticated in this request."));
            }

            var user = new UserGet
            {
                Id = (await _userInfoService.GetCurrentUserIdAsync()).ToString(),
                Name = await _userInfoService.GetCurrentUserNameAsync()
            };

            return Ok(user);
        }
    }
}
