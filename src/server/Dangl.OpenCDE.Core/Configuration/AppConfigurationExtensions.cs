using Dangl.Data.Shared.AspNetCore;
using Dangl.Identity.Client.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Dangl.OpenCDE.Core.Configuration
{
    public static class AppConfigurationExtensions
    {
        public static IApplicationBuilder ConfigureOpenCdeApp(this IApplicationBuilder app,
            IWebHostEnvironment environment,
            DanglIdentitySettings danglIdentitySettings)
        {
            danglIdentitySettings.Validate();

            app.UseForwardedHeaders();

            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials()
                .WithExposedHeaders("*"));

            app.UseResponseCompression();

            app.UseHttpsRedirection();

            app.UseOpenCdeVersionHeader();

            app.UseStaticFiles();

            app.UseDanglIdentityJwtTokenAuthentication(danglIdentitySettings.BaseUri, danglIdentitySettings.BaseUri, new System.Collections.Generic.List<string>
            {
                // The SignalR JavaScript library has a limitation, it can't set Bearer tokens via the 'Authorization' header
                // on the initial connection, so it sends the bearer token as a query parameter with the name 'access_token'
                // See here for more details: https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-5.0#bearer-token-authentication
                "access_token"
            });

            app.UseDanglIdentityJwtTokenUserInfoUpdater();

            app.UseOpenCdeSwaggerUi(danglIdentitySettings);

            app.UseHttpHeadToGetTransform();

            app.UseClientCompressionSupport();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                if (environment.IsDevelopment())
                {
                    spa.Options.SourcePath = "../dangl-opencde-ui";
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
                else
                {
                    spa.Options.DefaultPage = "/dist/index.html";
                }
            });

            return app;
        }
    }
}
