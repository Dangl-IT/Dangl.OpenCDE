using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class CallbackLink
    {
        [Required, JsonProperty("url")]
        public string Url { get; set; }

        [Required, JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
