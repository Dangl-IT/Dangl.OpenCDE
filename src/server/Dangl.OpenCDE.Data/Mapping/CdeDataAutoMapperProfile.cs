using AutoMapper;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Data.Dto.Projects;
using Dangl.OpenCDE.Data.Models;

namespace Dangl.OpenCDE.Data.Mapping
{
    public class CdeDataAutoMapperProfile : Profile
    {
        public CdeDataAutoMapperProfile()
        {
            CreateMap<Project, ProjectDto>();

            CreateMap<Document, DocumentDto>()
                .ForMember(dest => dest.ContentAvailable, opt => opt.MapFrom(src => src.FileId != null))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.File.FileName))
                .ForMember(dest => dest.FileSizeInBytes, opt => opt.MapFrom(src => src.File.SizeInBytes));
        }
    }
}
