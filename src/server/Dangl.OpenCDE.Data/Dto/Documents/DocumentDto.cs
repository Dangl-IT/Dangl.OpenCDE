using System;

namespace Dangl.OpenCDE.Data.Dto.Documents
{
    public class DocumentDto
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
