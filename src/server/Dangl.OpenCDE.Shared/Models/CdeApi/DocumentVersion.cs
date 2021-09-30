using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentVersion
    {
        [Required, JsonProperty("links")]
        public DocumentVersionLinks Links { get; set; }

        [Required, JsonProperty("version_number")]
        public string VersionNumber { get; set; }

        [Required, JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [Required, JsonProperty("title")]
        public string Title { get; set; }

        [Required, JsonProperty("file_description")]
        public FileDescription FileDescription { get; set; }
    }
}
