using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dangl.AspNetCore.FileHandling.Azure;

namespace Dangl.OpenCDE.Shared.Models.Controllers.Documents
{
    public class DocumentContentSasUploadResultGet
    {
        [Required]
        public SasUploadLink SasUploadLink { get; set; }

        [Required]
        public List<DocumentContentSasUploadResultHeaderGet> CustomHeaders { get; set; }
    }
}
