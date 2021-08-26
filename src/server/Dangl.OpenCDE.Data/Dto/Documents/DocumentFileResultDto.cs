using Dangl.AspNetCore.FileHandling.Azure;
using Dangl.Data.Shared;

namespace Dangl.OpenCDE.Data.Dto.Documents
{
    public class DocumentFileResultDto
    {
        public FileResultContainer FileResultContainer { get; set; }

        public SasDownloadLink SasDownloadLink { get; set; }
    }
}
