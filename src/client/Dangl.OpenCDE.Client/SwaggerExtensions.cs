using Dangl.OpenCDE.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag;
using System.Collections.Generic;
using System.Linq;

namespace Dangl.OpenCDE.Client
{
    public static class SwaggerExtensions
    {
        public static void AddCdeClientSwaggerServices(this IServiceCollection services)
        {
            services.AddOpenApiDocument(c =>
            {
                c.Description = "OpenCDE Client Specification";
                c.Version = VersionsService.Version;
                c.Title = $"OpenCDE Client {VersionsService.Version}";

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

                    var signalrTypes = typeof(Dangl.OpenCDE.Client.Models.OpenIdConnectAuthenticationResult)
                        .Assembly
                        .DefinedTypes
                        .Where(t => t.Namespace != null && t.Namespace
                            .StartsWith(typeof(Dangl.OpenCDE.Client.Models.OpenIdConnectAuthenticationResult).Namespace));
                    foreach (var type in signalrTypes)
                    {
                        if (!x.Definitions.ContainsKey(type.Name))
                        {
                            x.Definitions.Add(type.Name, JsonSchema.FromType(type, new NJsonSchema.Generation.JsonSchemaGeneratorSettings
                            {
                                SerializerSettings = c.SerializerSettings
                            }));
                        }
                    }
                };
            });
        }

        /// <summary>
        /// Adds the PfeifferAVA Swagger endpoints
        /// </summary>
        /// <param name="app"></param>
        /// <param name="danglIdentitySettings"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCdeClientSwaggerUi(this IApplicationBuilder app)
        {
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

            return app;
        }
    }
}
