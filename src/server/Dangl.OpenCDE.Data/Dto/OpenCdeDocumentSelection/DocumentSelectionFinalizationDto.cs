using System;

namespace Dangl.OpenCDE.Data.Dto.OpenCdeDocumentSelection
{
    public class DocumentSelectionFinalizationDto
    {
        public Guid SelectionId { get; set; }

        public string ClientState { get; set; }

        public string ClientCallbackUrl { get; set; }
    }
}
