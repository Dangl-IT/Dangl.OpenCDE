using Newtonsoft.Json;

namespace Dangl.OpenCDE.Shared.Models.Foundations
{
    public class UserGet
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
