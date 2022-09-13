using System;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Data.Models
{
    public class PendingOpenCdeUploadFile
    {
        public Guid Id { get; set; }    

        public Guid UploadSessionId { get; set; }

        public OpenCdeDocumentUploadSession UploadSession { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string SessionFileId { get; set; }

        public Guid? LinkedCdeDocumentId { get; set; }

        public Document LinkedCdeDocument { get; set; }

        public bool IsCancelled { get; set; } = false;
    }
}
