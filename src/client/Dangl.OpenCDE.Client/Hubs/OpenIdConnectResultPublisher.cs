using Dangl.OpenCDE.Client.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Client.Hubs
{
    public class OpenIdConnectResultPublisher
    {
        private readonly IHubContext<CdeClientHub> _hubContext;

        public OpenIdConnectResultPublisher(IHubContext<CdeClientHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task InformClientsAboutAuthenticationFailureAsync(string state)
        {
            await _hubContext
                .Clients
                .All
                .SendAsync("OpenIdConnectCallback", new OpenIdConnectAuthenticationResult
                {
                    IsSuccess = false,
                    ClientState = state
                });
        }

        public async Task InformClientsAboutAuthenticationSuccess(string state, string jwtToken, int expiresIn)
        {
            var expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn).ToUnixTimeSeconds();

            await _hubContext
                .Clients
                .All
                .SendAsync("OpenIdConnectCallback", new OpenIdConnectAuthenticationResult
                {
                    IsSuccess = true,
                    JwtToken = jwtToken,
                    ExpiresAt = expiresAt,
                    ClientState = state
                });
        }
    }
}
