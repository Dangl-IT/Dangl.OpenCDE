using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Client.Models
{
    public class DocumentSelectionInitializationParameters
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string ClientState { get; set; }

        [Required]
        public string OpenCdeBaseUrl { get; set; }
    }
}
