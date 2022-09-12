using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Dangl.OpenCDE.Core.Configuration
{
    public class OpenCdeApisOnlyOperationProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var validTypes = new[]
            {
                    typeof(Dangl.OpenCDE.Core.Controllers.Foundations.AuthenticationController),
                    typeof(Dangl.OpenCDE.Core.Controllers.Foundations.CurrentUserController),
                    typeof(Dangl.OpenCDE.Core.Controllers.Foundations.VersionsController),
                    typeof(Dangl.OpenCDE.Core.Controllers.CdeApi.OpenCdeDownloadController),
                    typeof(Dangl.OpenCDE.Core.Controllers.CdeApi.OpenCdeDownloadIntegrationController),
                    typeof(Dangl.OpenCDE.Core.Controllers.CdeApi.OpenCdeQueryController),
                    typeof(Dangl.OpenCDE.Core.Controllers.CdeApi.OpenCdeUploadController),
                    typeof(Dangl.OpenCDE.Core.Controllers.CdeApi.OpenCdeUploadIntegrationController)
                };

            var shouldProcess = validTypes.Contains(context.ControllerType);

            return shouldProcess;
        }
    }
}
