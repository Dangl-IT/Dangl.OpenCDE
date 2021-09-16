using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Foundations
{
    public class VersionsGet
    {
        [Required]
        [JsonProperty("versions")]
        public List<VersionGet> Versions { get; set; }
    }
}
