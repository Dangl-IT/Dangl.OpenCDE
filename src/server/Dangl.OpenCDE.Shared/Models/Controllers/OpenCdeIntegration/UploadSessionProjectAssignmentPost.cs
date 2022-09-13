using System;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Controllers.OpenCdeIntegration
{
    public class UploadSessionProjectAssignmentPost
    {
        [Required]
        public Guid ProjectId { get; set; }
    }
}
