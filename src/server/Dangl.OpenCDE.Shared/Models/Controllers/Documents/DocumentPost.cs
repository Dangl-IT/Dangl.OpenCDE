using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Controllers.Documents
{
    public class DocumentPost
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
