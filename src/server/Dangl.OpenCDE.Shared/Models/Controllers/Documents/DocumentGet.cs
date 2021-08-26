using System;

namespace Dangl.OpenCDE.Shared.Models.Controllers.Documents
{
    public class DocumentGet
    {
        public Guid Id { get; set; }

        public Guid ProjectId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTimeOffset CreatedAtUtc { get; set; }

        public bool ContentAvailable { get; set; }

        public string FileName { get; set; }

        public long? FileSizeInBytes { get; set; }
    }
}
