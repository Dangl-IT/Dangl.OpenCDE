using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Client.Models
{
    public class DocumentUploadInitializationParameters
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string ClientState { get; set; }

        [Required]
        public string OpenCdeBaseUrl { get; set; }

        [Required]
        public List<DocumentUploadInitializationFile> Files { get; set; }
    }
}
