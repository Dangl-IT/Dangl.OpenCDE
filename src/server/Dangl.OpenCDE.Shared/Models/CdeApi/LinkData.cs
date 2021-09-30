using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class LinkData
    {
        [Required, JsonProperty("url")]
        public string Url { get; set; }
    }
}
