using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Controllers.Documents
{
    public class DocumentContentPreparationPost
    {
        [Required]
        public string FileName { get; set; }

        [Required]
        public string ContentType { get; set; }

        [Required]
        public long SizeInBytes { get; set; }
    }
}
