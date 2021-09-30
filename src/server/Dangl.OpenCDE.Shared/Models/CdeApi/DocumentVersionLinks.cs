using Newtonsoft.Json;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentVersionLinks
    {
        [JsonProperty("document_version")]
        public LinkData DocumentVersion { get; set; }

        [JsonProperty("document_version_metadata")]
        public LinkData DocumentVersionMetadata { get; set; }

        [JsonProperty("document_version_download")]
        public LinkData DocumentVersionDownload { get; set; }

        [JsonProperty("document_versions")]
        public LinkData DocumentVersions { get; set; }
    }
}
