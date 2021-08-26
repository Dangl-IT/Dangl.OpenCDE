using System;

namespace Dangl.OpenCDE.Shared.Models.Controllers.Projects
{
    public class ProjectGet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid IdenticonId { get; set; }
    }
}
