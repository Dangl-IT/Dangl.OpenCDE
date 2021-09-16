using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Foundations
{
    public class VersionGet
    {
        [Required]
        [JsonProperty("api_id")]
        public string ApiId { get; set; }

        [Required]
        [JsonProperty("version_id")]
        public string VersionId { get; set; }

        [JsonProperty("detailed_version")]
        public string DetailedVersion { get; set; }

        [JsonProperty("api_base_url")]
        public string ApiBaseUrl { get; set; }
    }
}
