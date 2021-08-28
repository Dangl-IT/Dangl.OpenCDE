using System;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Controllers.Documents
{
    public class DocumentGet
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTimeOffset CreatedAtUtc { get; set; }

        [Required]
        public bool ContentAvailable { get; set; }

        public string FileName { get; set; }

        public long? FileSizeInBytes { get; set; }
    }
}
