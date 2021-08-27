using AutoMapper;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Data.Dto.Projects;
using Dangl.OpenCDE.Shared.Models.Controllers.Documents;
using Dangl.OpenCDE.Shared.Models.Controllers.Projects;

namespace Dangl.OpenCDE.Core.Mapping
{
    public class CdeAutoMapperProfile : Profile
    {
        public CdeAutoMapperProfile()
        {
            CreateMap<ProjectDto, ProjectGet>();
            CreateMap<ProjectPost, CreateProjectDto>();

            CreateMap<DocumentDto, DocumentGet>();
            CreateMap<DocumentPost, CreateDocumentDto>();
        }
    }
}
