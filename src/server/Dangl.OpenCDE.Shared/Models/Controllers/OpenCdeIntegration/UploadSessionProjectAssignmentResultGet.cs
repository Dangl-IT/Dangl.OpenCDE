using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Controllers.OpenCdeIntegration
{
    public class UploadSessionProjectAssignmentResultGet
    {
        [Required]
        public string ClientCallbackUrl { get; set; }
    }
}
