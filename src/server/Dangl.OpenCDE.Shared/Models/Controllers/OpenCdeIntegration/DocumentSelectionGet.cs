using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Controllers.OpenCdeIntegration
{
    public class DocumentSelectionGet
    {
        [Required]
        public string CallbackUrl { get; set; }
    }
}
