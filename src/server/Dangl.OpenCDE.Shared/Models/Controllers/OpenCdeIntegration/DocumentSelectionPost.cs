using System;
using System.ComponentModel.DataAnnotations;

namespace Dangl.OpenCDE.Shared.Models.Controllers.OpenCdeIntegration
{
    public class DocumentSelectionPost
    {
        [Required]
        public Guid DocumentId { get; set; }
    }
}
