using Dangl.OpenCDE.Shared;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;

namespace Dangl.OpenCDE.Core.Configuration
{
    /// <summary>
    /// Extensions for the app pipeline
    /// </summary>
    public static class OpenCdeVersionHeaderAppExtensions
    {
        /// <summary>
        /// This appends the X-DANGL-OPEN-CDE-VERSION header to all responses
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseOpenCdeVersionHeader(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context?.Response.Headers.TryAdd("X-DANGL-OPEN-CDE-VERSION", VersionsService.Version);
                await next();
            });

            return app;
        }
    }
}
