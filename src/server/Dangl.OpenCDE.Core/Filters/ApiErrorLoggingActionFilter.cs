using Dangl.Data.Shared;
using Dangl.Data.Shared.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dangl.OpenCDE.Core.Filters
{
    public class ApiErrorLoggingActionFilter : ActionFilterAttribute
    {
        private readonly ILogger<ApiErrorLoggingActionFilter> _logger;

        public ApiErrorLoggingActionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ApiErrorLoggingActionFilter>();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context?.Result is ObjectResult objectResult
                && objectResult?.Value is ApiError apiError)
            {
                var jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.ConfigureDefaultJsonSerializerSettings();
                jsonSerializerSettings.Formatting = Formatting.Indented;
                var apiErrorJson = JsonConvert.SerializeObject(apiError, jsonSerializerSettings);
                _logger.LogInformation($@"This request returned an API Error with status code {objectResult.StatusCode}:
{apiErrorJson}");
            }
        }
    }
}
