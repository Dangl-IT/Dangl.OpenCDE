using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models;

namespace Dangl.OpenCDE.Client.Models
{
    public class OpenCdeUploadTask
    {
        public DocumentsToUpload CdeUploadInstructions { get; set; }

        public DocumentUploadInitializationParameters ClientFileData { get; set; }
    }
}
