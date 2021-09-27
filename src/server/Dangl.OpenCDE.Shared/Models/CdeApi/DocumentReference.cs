using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public class DocumentReference
    {
        [Required, JsonProperty("links")]
        public DocumentReferenceLinks Links { get; set; }

        [Required, JsonProperty("version")]
        public string Version { get; set; }

        [Required, JsonProperty("version_date")]
        public DateTimeOffset VersionDate { get; set; }

        [Required, JsonProperty("title")]
        public string Title { get; set; }

        [Required, JsonProperty("file_description")]
        public FileDescription FileDescription { get; set; }
    }
}
