using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Controllers.FrontendConfig
{
    public class FrontendConfigGet
    {
        [Required]
        public string DanglIconsBaseUrl { get; set; }

        [Required]
        public string DanglIdentityUrl { get; set; }

        public string ApplicationInsightsInstrumentationKey { get; set; }

        [Required]
        public string Environment { get; set; }

        [Required]
        public string DanglIdentityClientId { get; set; }

        [Required]
        public string RequiredScope { get; set; }
    }
}
