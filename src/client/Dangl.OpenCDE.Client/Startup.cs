using Dangl.OpenCDE.Client.Hubs;
using Dangl.OpenCDE.Client.Services;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Client
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSignalR();
            services.AddHttpClient();
            services.AddTransient<OpenIdConnectResultPublisher>();
            services.AddCdeClientSwaggerServices();
            services.AddSingleton<OpenIdConnectCache>();
            services.AddSingleton<OpenIdAuthenticationRequestHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseStaticFiles();

            app.UseCdeClientSwaggerUi();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CdeClientHub>("/hubs/cde-client");
            });

            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                {
                    spa.Options.SourcePath = "../dangl-opencde-client-ui";
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4201");
                }
                else
                {
                    spa.Options.DefaultPage = "/dist/index.html";
                }
            });

            Task.Run(async () =>
            {
                var browserWindowOptions = new BrowserWindowOptions
                {
                    Title = "Dangl.OpenCDE.Client",
                    Icon = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/icon_512.png")
                };

                await Electron.WindowManager.CreateWindowAsync(browserWindowOptions);
            });
        }
    }
}
