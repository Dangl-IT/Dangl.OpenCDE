using NSwag.Generation.Processors.Contexts;
using NSwag.Generation.Processors;
using System.Linq;

namespace Dangl.OpenCDE.Client
{
    public class SignalRTypesProcessor : IDocumentProcessor
    {
        public void Process(DocumentProcessorContext context)
        {
            var signalrTypes = typeof(Dangl.OpenCDE.Client.Models.OpenIdConnectAuthenticationResult)
                        .Assembly
                        .DefinedTypes
                        .Where(t => t.Namespace != null && t.Namespace
                            .StartsWith(typeof(Dangl.OpenCDE.Client.Models.OpenIdConnectAuthenticationResult).Namespace));
            foreach (var type in signalrTypes)
            {
                if (!context.SchemaResolver.HasSchema(type, false))
                {
                    context.SchemaGenerator.Generate(type, context.SchemaResolver);
                }
            }
        }
    }
}
