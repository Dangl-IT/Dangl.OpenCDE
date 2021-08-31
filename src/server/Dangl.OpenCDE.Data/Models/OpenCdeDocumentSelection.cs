using System;

namespace Dangl.OpenCDE.Data.Models
{
    public class OpenCdeDocumentSelection
    {
        public Guid Id { get; set; }

        public Guid DocumentId { get; set; }

        public Document Document { get; set; }

        public Guid UserId { get; set; }

        public CdeUser User { get; set; }
    }
}
