using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Controllers.Documents
{
    public class DocumentContentSasUploadResultHeaderGet
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
