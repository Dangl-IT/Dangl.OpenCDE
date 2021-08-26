using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dangl.Data.Shared;
using Dangl.Data.Shared.QueryUtilities;
using Dangl.OpenCDE.Data.Dto.Projects;
using Dangl.OpenCDE.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.Repository
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly CdeDbContext _context;
        private readonly IMapper _mapper;

        public ProjectsRepository(CdeDbContext context,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IQueryable<ProjectDto> GetAllProjects(string filter)
        {
            var filteredQuery = _context
                .Projects
                .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
                .Filter(filter, text => p =>
                    EF.Functions.Like(p.Name, $"%{text}%")
                    || EF.Functions.Like(p.Description, $"%{text}%"),
                        transformFilterToLowercase: true);

            return filteredQuery;
        }

        public async Task<RepositoryResult<ProjectDto>> CreateProjectAsync(CreateProjectDto project)
        {
            if (string.IsNullOrWhiteSpace(project?.Name))
            {
                return RepositoryResult<ProjectDto>.Fail("The project name can not be empty.");
            }

            if (project.Name.Length > 400)
            {
                return RepositoryResult<ProjectDto>.Fail("The project name can not be longer than 400 characters.");
            }

            var dbProject = new Project
            {
                Name = project.Name.Trim(),
                IdenticonId = Guid.NewGuid(),
                Description = project.Description,
            };
            _context.Projects.Add(dbProject);

            await _context.SaveChangesAsync();
            var dto = _mapper.Map<ProjectDto>(dbProject);

            return RepositoryResult<ProjectDto>.Success(dto);
        }

        public Task<bool> CheckIfProjectExistsAsync(Guid projectId)
        {
            return _context.Projects.AnyAsync(p => p.Id == projectId);
        }
    }
}
