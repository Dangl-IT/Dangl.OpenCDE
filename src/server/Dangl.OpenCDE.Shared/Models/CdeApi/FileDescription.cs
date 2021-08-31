using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class FileDescription
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required, JsonProperty("size_in_bytes")]
        public long FileSizeInBytes { get; set; }
    }
}
