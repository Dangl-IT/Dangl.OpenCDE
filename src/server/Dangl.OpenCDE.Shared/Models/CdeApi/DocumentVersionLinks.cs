using Newtonsoft.Json;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentVersionLinks
    {
        [JsonProperty("self")]
        public LinkData Self { get; set; }
    }
}
