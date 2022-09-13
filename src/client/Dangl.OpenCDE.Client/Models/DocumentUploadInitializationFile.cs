using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Client.Models
{
    public class DocumentUploadInitializationFile
    {
        [Required]
        public string SessionFileId { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public long FileSizeInBytes { get; set; }

        [Required]
        public string FilePath { get; set; }
    }
}
