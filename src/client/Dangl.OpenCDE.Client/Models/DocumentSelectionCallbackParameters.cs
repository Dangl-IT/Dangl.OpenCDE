using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Client.Models
{
    public class DocumentSelectionCallbackParameters
    {
        [Required]
        public string CallbackUrl { get; set; }
    }
}
