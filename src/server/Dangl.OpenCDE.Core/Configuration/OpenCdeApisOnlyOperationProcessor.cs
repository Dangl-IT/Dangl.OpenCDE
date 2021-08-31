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
                    typeof(Dangl.OpenCDE.Core.Controllers.CdeApi.DocumentSelectionController),
                    typeof(Dangl.OpenCDE.Core.Controllers.OpenCdeIntegrationController),
                };

            var shouldProcess = validTypes.Contains(context.ControllerType);

            return shouldProcess;
        }
    }
}
