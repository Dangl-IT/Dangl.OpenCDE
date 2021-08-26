using System;

namespace Dangl.OpenCDE.Data.Dto.Projects
{
    public class ProjectDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid IdenticonId { get; set; }

        public DateTimeOffset CreatedAtUtc { get; set; }

        public string Description { get; set; }
    }
}
