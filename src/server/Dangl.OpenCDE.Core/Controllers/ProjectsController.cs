using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dangl.Data.Shared;
using Dangl.OpenCDE.Data.Dto.Projects;
using Dangl.OpenCDE.Data.Repository;
using Dangl.OpenCDE.Shared.Models.Controllers.Projects;
using LightQuery.Client;
using LightQuery.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Core.Controllers
{
    [Route("api/projects")]
    public class ProjectsController : CdeAppControllerBase
    {
        private readonly IProjectsRepository _projectsRepository;
        private readonly IMapper _mapper;

        public ProjectsController(IProjectsRepository projectsRepository,
            IMapper mapper)
        {
            _projectsRepository = projectsRepository;
            _mapper = mapper;
        }

        [AsyncLightQuery(forcePagination: true)]
        [HttpGet("")]
        [ProducesResponseType(typeof(PaginationResult<ProjectGet>), (int)HttpStatusCode.OK)]
        public IActionResult GetAllProjects([FromQuery] string filter = null)
        {
            var projects = _projectsRepository.GetAllProjects(filter)
                .ProjectTo<ProjectGet>(_mapper.ConfigurationProvider);

            return Ok(projects);
        }

        [HttpGet("{projectId}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProjectGet), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProjectGet>> GetProjectByIdAsync(Guid projectId)
        {
            var project = await _projectsRepository
                .GetAllProjects(null)
                .ProjectTo<ProjectGet>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProjectGet), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateProjectAsync(ProjectPost model)
        {
            var repoModel = _mapper.Map<CreateProjectDto>(model);

            var creationResult = await _projectsRepository.CreateProjectAsync(repoModel);

            return CreatedFromRepositoryResult<ProjectDto, ProjectGet>(creationResult,
                nameof(GetProjectByIdAsync),
                () => new { projectId = creationResult.Value.Id },
                _mapper);
        }
    }
}
