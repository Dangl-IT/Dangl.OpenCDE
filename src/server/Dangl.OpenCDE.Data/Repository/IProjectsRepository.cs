using Dangl.Data.Shared;
using Dangl.OpenCDE.Data.Dto.Projects;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.Repository
{
    public interface IProjectsRepository
    {
        /// <summary>
        /// The userId parameter is used to create the <see cref="ProjectDto.LastAccessedAtUtc"/>
        /// dynamically for the currently requesting user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="filter"></param>
        /// <param name="status"></param>
        /// <param name="includeArchived"></param>
        /// <returns></returns>
        IQueryable<ProjectDto> GetAllProjects(string filter);

        /// <summary>
        /// The <see cref="Project.ProjectYear"/> and <see cref="ProjectDto.ProjectNumber"/> are
        /// automatically generated based on the current year a a distinct sequence for each year.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<RepositoryResult<ProjectDto>> CreateProjectAsync(CreateProjectDto project);

        /// <summary>
        /// This will simply check if a project with the given id is present
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<bool> CheckIfProjectExistsAsync(Guid projectId);
    }
}
