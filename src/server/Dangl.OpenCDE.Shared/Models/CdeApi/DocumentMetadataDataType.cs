using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentMetadataDataType
    {
        [Required, JsonProperty("type")]
        public DocumentMetdataEntryType Type { get; set; }

        [Required, JsonProperty("required")]
        public bool IsRequired { get; set; }

        [JsonProperty("enumValues")]
        public List<string> EnumValues { get; set; }
    }
}
