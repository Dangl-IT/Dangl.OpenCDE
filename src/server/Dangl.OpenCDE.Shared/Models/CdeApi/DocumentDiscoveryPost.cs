using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentDiscoveryPost
    {
        [Required, JsonProperty("callback")]
        public CallbackLink CallbackLink { get; set; }

        [JsonProperty("state")]
        public string ClientState { get; set; }

        [JsonProperty("project_id")]
        public Guid? ProjectId { get; set; }
    }
}
