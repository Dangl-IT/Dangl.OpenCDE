using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentVersions
    {
        [Required, JsonProperty("documents")]
        public List<DocumentVersion> DocumentVersion { get; set; }
    }
}
