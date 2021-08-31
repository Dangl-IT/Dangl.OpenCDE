using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class LinkData
    {
        [Required, JsonProperty("href")]
        public string Href { get; set; }
    }
}
