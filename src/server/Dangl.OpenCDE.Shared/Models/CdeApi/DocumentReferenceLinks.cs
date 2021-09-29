using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentReferenceLinks
    {
        [Required, JsonProperty("self")]
        public LinkData Self { get; set; }

        [Required, JsonProperty("metadata")]
        public LinkData Metadata { get; set; }

        [Required, JsonProperty("versions")]
        public LinkData Versions { get; set; }

        [Required, JsonProperty("download")]
        public LinkData Download { get; set; }
    }
}
