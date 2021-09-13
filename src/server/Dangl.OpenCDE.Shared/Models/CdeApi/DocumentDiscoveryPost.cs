using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentDiscoveryPost
    {
        [Required, JsonProperty("callback")]
        public CallbackLink CallbackLink { get; set; }

        [JsonProperty("selection_context")]
        public Guid? SelectionContext { get; set; }
    }
}
