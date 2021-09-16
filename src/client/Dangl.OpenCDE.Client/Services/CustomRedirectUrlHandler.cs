using Dangl.OpenCDE.Client.Hubs;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Client.Services
{
    public class CustomRedirectUrlHandler
    {
        private static Dictionary<int, CustomRedirectUrlHandler> _handlersByPort = new Dictionary<int, CustomRedirectUrlHandler>();
        private readonly Uri _customRedirectUrl;
        private IWebHost _webHost;
        private readonly IServiceScope _serviceScope;

        public CustomRedirectUrlHandler(string customRedirectUrl,
            IServiceScope serviceScope)
        {
            if (!Uri.TryCreate(customRedirectUrl, UriKind.Absolute, out var parsedUri))
            {
                throw new ArgumentException($"The {nameof(customRedirectUrl)} needs to be an absolute url", nameof(customRedirectUrl));
            }

            _customRedirectUrl = parsedUri;
            _serviceScope = serviceScope;
        }

        public async Task StartListeningAsync()
        {
            var port = _customRedirectUrl.Port;
            if (_handlersByPort.TryGetValue(port, out var existingHandler))
            {
                await existingHandler.StopListeningAsync();
            }
            _handlersByPort[port] = this;

            if (_webHost != null)
            {
                throw new Exception("There is already a listener running.");
            }

            _webHost = BuildWebHost(port);

            await _webHost.StartAsync();
        }

        public async Task StopListeningAsync()
        {
            await _webHost.StopAsync();
        }

        private IWebHost BuildWebHost(int port)
        {
            return WebHost.CreateDefaultBuilder()
                .UseUrls($"http://+:{port}")
                .Configure(app =>
                {
                    app.Use(async (context, next) =>
                    {
                        context.Response.StatusCode = 200;
                        context.Response.Headers.TryAdd("Content-Type", "text/html");

                        using var scope = _serviceScope.ServiceProvider.CreateScope();
                        var openIdCache = scope.ServiceProvider.GetRequiredService<OpenIdConnectCache>();
                        var resultsPublisher = scope.ServiceProvider.GetRequiredService<OpenIdConnectResultPublisher>();
                        var clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

                        var openIdHandler = new OpenIdAuthenticationRequestHandler(openIdCache,
                            resultsPublisher,
                            clientFactory);

                        var queryCode = string.Empty;
                        if (context.Request.Query.TryGetValue("code", out var queryCodeValue))
                        {
                            queryCode = queryCodeValue.First();
                        }

                        var openIdResponseContent = await openIdHandler.HandleOpenIdCodeResponse(context, queryCode);
                        await context.Response.WriteAsync(openIdResponseContent);
                    });
                })
                .Build();
        }
    }
}
