using Dangl.OpenCDE.Shared;
using LightQuery.NSwag;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using System.Collections.Generic;
using System.Linq;

namespace Dangl.OpenCDE.Core.Configuration
{
    public static class SwaggerExtensions
    {
        public static void AddOpenCdeSwaggerServices(this IServiceCollection services,
            OpenCdeSettings openCdeSettings)
        {
            AddGeneralSwaggerDocument(services, openCdeSettings);
            AddOpenCdeDocumentsApiSwaggerDocument(services, openCdeSettings);
        }

        private static void AddGeneralSwaggerDocument(IServiceCollection services,
            OpenCdeSettings openCdeSettings)
        {
            services.AddOpenApiDocument(c =>
            {
                c.Description = "Dangl.OpenCDE API Specification";
                c.Version = VersionsService.Version;
                c.Title = $"API {VersionsService.Version}";

                var requiredScope = openCdeSettings.DanglIdentitySettings.RequiredScope;
                var danglIdentityBaseUrl = openCdeSettings.DanglIdentitySettings.BaseUri;
                c.DocumentProcessors.Add(new SecurityDefinitionAppender("Dangl.Identity",
                    new[] { requiredScope },
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.OAuth2,
                        Description = "OpenCDE API Access",
                        Flow = OpenApiOAuth2Flow.Implicit,
                        AuthorizationUrl = danglIdentityBaseUrl.TrimEnd('/') + "/connect/authorize",
                        TokenUrl = danglIdentityBaseUrl.TrimEnd('/') + "/connect/token",
                        Scopes = new Dictionary<string, string> { { requiredScope, "Access to the OpenCDE API" } }
                    }));

                c.OperationProcessors.Add(new OperationSecurityScopeProcessor("Dangl.Identity"));
                c.OperationProcessors.Add(new LightQueryOperationsProcessor());

                c.PostProcess = (x) =>
                {
                    // Some enum classes use multiple integer values for the same value, e.g.
                    // System.Net.HttpStatusCode uses these:
                    // RedirectKeepVerb = 307
                    // TemporaryRedirect = 307
                    // MVC is configured to use the StringEnumConverter, and NJsonSchema errorenously
                    // outputs the duplicates. For the example above, the value 'TemporaryRedirect' is
                    // serialized twice, 'RedirectKeepVerb' is missing.
                    // The following post process action should remove duplicated enums
                    // See https://github.com/RSuter/NJsonSchema/issues/800 for more information
                    foreach (var enumType in x.Definitions.Select(d => d.Value).Where(d => d.IsEnumeration))
                    {
                        var distinctValues = enumType.Enumeration.Distinct().ToList();
                        enumType.Enumeration.Clear();
                        foreach (var distinctValue in distinctValues)
                        {
                            enumType.Enumeration.Add(distinctValue);
                        }
                    }

                    // TODO CHECK THIS WORKAROUND WHEN THE RESOLUTION ON GITHUB WAS FOUND:
                    // https://github.com/RicoSuter/NSwag/issues/2119
                    var elementProperties = x.Definitions.Select(d => d.Value).SelectMany(v => v.Properties);
                    var additionalToInheritedProperties = x.Definitions.SelectMany(d => d.Value.AllOf).SelectMany(a => a.Properties);
                    foreach (var property in elementProperties.Concat(additionalToInheritedProperties).Select(p => p.Value))
                    {
                        if (property.AllOf.Count == 1 && property.AllOf.Single().Reference != null)
                        {
                            property.Reference = property.AllOf.Single().Reference;
                            property.AllOf.Clear();
                        }
                    }
                };
            });
        }

        private static void AddOpenCdeDocumentsApiSwaggerDocument(IServiceCollection services,
            OpenCdeSettings openCdeSettings)
        {
            services.AddOpenApiDocument(c =>
            {
                c.Description = "OpenCDE Documents API Specification";
                c.Version = VersionsService.Version;
                c.Title = $"API {VersionsService.Version}";

                c.DocumentName = "OpenCDE";

                var requiredScope = openCdeSettings.DanglIdentitySettings.RequiredScope;
                var danglIdentityBaseUrl = openCdeSettings.DanglIdentitySettings.BaseUri;
                c.DocumentProcessors.Add(new SecurityDefinitionAppender("Dangl.Identity",
                    new[] { requiredScope },
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.OAuth2,
                        Description = "OpenCDE API Access",
                        Flow = OpenApiOAuth2Flow.Implicit,
                        AuthorizationUrl = danglIdentityBaseUrl.TrimEnd('/') + "/connect/authorize",
                        TokenUrl = danglIdentityBaseUrl.TrimEnd('/') + "/connect/token",
                        Scopes = new Dictionary<string, string> { { requiredScope, "Access to the OpenCDE API" } }
                    }));

                c.OperationProcessors.Add(new OperationSecurityScopeProcessor("Dangl.Identity"));
                // We're filtering out all the non-bSI API types here
                c.OperationProcessors.Insert(0, new OpenCdeApisOnlyOperationProcessor());

                c.PostProcess = (x) =>
                {
                    // Some enum classes use multiple integer values for the same value, e.g.
                    // System.Net.HttpStatusCode uses these:
                    // RedirectKeepVerb = 307
                    // TemporaryRedirect = 307
                    // MVC is configured to use the StringEnumConverter, and NJsonSchema errorenously
                    // outputs the duplicates. For the example above, the value 'TemporaryRedirect' is
                    // serialized twice, 'RedirectKeepVerb' is missing.
                    // The following post process action should remove duplicated enums
                    // See https://github.com/RSuter/NJsonSchema/issues/800 for more information
                    foreach (var enumType in x.Definitions.Select(d => d.Value).Where(d => d.IsEnumeration))
                    {
                        var distinctValues = enumType.Enumeration.Distinct().ToList();
                        enumType.Enumeration.Clear();
                        foreach (var distinctValue in distinctValues)
                        {
                            enumType.Enumeration.Add(distinctValue);
                        }
                    }

                    // TODO CHECK THIS WORKAROUND WHEN THE RESOLUTION ON GITHUB WAS FOUND:
                    // https://github.com/RicoSuter/NSwag/issues/2119
                    var elementProperties = x.Definitions.Select(d => d.Value).SelectMany(v => v.Properties);
                    var additionalToInheritedProperties = x.Definitions.SelectMany(d => d.Value.AllOf).SelectMany(a => a.Properties);
                    foreach (var property in elementProperties.Concat(additionalToInheritedProperties).Select(p => p.Value))
                    {
                        if (property.AllOf.Count == 1 && property.AllOf.Single().Reference != null)
                        {
                            property.Reference = property.AllOf.Single().Reference;
                            property.AllOf.Clear();
                        }
                    }
                };
            });
        }

        /// <summary>
        /// Adds the OpenCDE Swagger endpoints
        /// </summary>
        /// <param name="app"></param>
        /// <param name="danglIdentitySettings"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseOpenCdeSwaggerUi(this IApplicationBuilder app, DanglIdentitySettings danglIdentitySettings)
        {
            void ConfigureDanglIdentity(SwaggerUiSettings settings)
            {
                settings.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = danglIdentitySettings.ClientId,
                    AppName = "Dangl.OpenCDE"
                };
            }

            app.UseOpenApi(c =>
            {
                c.Path = "/swagger/swagger.json";
                c.PostProcess = (doc, _) =>
                {
                    // This makes sure that Azure warmup requests that are sent via Http instead of Https
                    // don't set the document schema to http only
                    doc.Schemes = new List<OpenApiSchema> { OpenApiSchema.Https };
                };
            });

            app.UseSwaggerUi(settings =>
            {
                ConfigureDanglIdentity(settings);

                settings.DocumentPath = "/swagger/swagger.json";
                settings.Path = "/swagger";
            });

            app.UseOpenApi(c =>
            {
                c.DocumentName = "OpenCDE";
                c.Path = "/swagger/opencde.json";
                c.PostProcess = (doc, _) =>
                {
                    // This makes sure that Azure warmup requests that are sent via Http instead of Https
                    // don't set the document schema to http only
                    doc.Schemes = new List<OpenApiSchema> { OpenApiSchema.Https };
                };
            });

            app.UseOpenApi(c =>
            {
                c.DocumentName = "OpenCDE";
                c.Path = "/swagger/opencde.yaml";
                c.PostProcess = (doc, _) =>
                {
                    // This makes sure that Azure warmup requests that are sent via Http instead of Https
                    // don't set the document schema to http only
                    doc.Schemes = new List<OpenApiSchema> { OpenApiSchema.Https };
                };
            });

            app.UseSwaggerUi(settings =>
            {
                ConfigureDanglIdentity(settings);

                settings.DocumentPath = "/swagger/opencde.json";
                settings.Path = "/swagger-opencde";
            });

            return app;
        }
    }
}
