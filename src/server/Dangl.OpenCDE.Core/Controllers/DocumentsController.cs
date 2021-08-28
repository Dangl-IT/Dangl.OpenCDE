using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dangl.Data.Shared;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Data.Repository;
using Dangl.OpenCDE.Shared.Models.Controllers.Documents;
using LightQuery.Client;
using LightQuery.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Core.Controllers
{
    [Route("api/projects/{projectId}/documents")]
    public class DocumentsController : CdeAppControllerBase
    {
        private readonly IDocumentsRepository _documentsRepository;
        private readonly IProjectsRepository _projectsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DocumentsController(IDocumentsRepository documentsRepository,
            IProjectsRepository projectsRepository,
            IMapper mapper,
            ILoggerFactory loggerFactory)
        {
            _documentsRepository = documentsRepository;
            _projectsRepository = projectsRepository;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<DocumentsController>();
        }

        [AsyncLightQuery(forcePagination: true)]
        [HttpGet("")]
        [ProducesResponseType(typeof(PaginationResult<DocumentGet>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllDocumentsForProjectAsync(Guid projectId, [FromQuery] string filter = null)
        {
            if (!await _projectsRepository.CheckIfProjectExistsAsync(projectId))
            {
                return NotFound();
            }

            var documents = _documentsRepository
                .GetAllDocumentsForProject(projectId, filter)
                .ProjectTo<DocumentGet>(_mapper.ConfigurationProvider);

            return Ok(documents);
        }

        [HttpGet("{documentId}")]
        [ProducesResponseType(typeof(DocumentGet), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetDocumentById(Guid projectId, Guid documentId)
        {
            if (!await _projectsRepository.CheckIfProjectExistsAsync(projectId))
            {
                return NotFound();
            }

            var document = await _documentsRepository
                .GetAllDocumentsForProject(projectId, null)
                .Where(d => d.Id == documentId)
                .ProjectTo<DocumentGet>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (document == null)
            {
                return NotFound();
            }

            return Ok(document);
        }

        [HttpGet("{documentId}/content")]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Redirect)]
        public async Task<IActionResult> DownloadDocumentAsync(Guid projectId, Guid documentId)
        {
            if (!await _projectsRepository.CheckIfProjectExistsAsync(projectId))
            {
                return NotFound();
            }

            var documentDownloadResult = await _documentsRepository
                .GetDocumentDataAsync(documentId);

            if (!documentDownloadResult.IsSuccess)
            {
                return BadRequest(new ApiError(documentDownloadResult.ErrorMessage));
            }

            if (documentDownloadResult.Value.FileResultContainer != null)
            {
                var file = documentDownloadResult.Value.FileResultContainer;
                return File(file.Stream, file.MimeType, file.FileName);
            }
            else if (documentDownloadResult.Value.SasDownloadLink != null)
            {
                return Redirect(documentDownloadResult.Value.SasDownloadLink.DownloadLink);
            }
            else
            {
                _logger.LogError("Encountered invalid repository response when trying to download a document");
                return BadRequest(new ApiError("Internal error, please contact support."
                    + Environment.NewLine
                    + $"Correlation Id: {HttpContext.TraceIdentifier}"));
            }

            throw new NotImplementedException();
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(DocumentGet), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UploadDocumentMetadataForProjectAsync(Guid projectId,
            DocumentPost documentData)
        {
            if (!await _projectsRepository.CheckIfProjectExistsAsync(projectId))
            {
                return NotFound();
            }

            var creationDto = _mapper.Map<CreateDocumentDto>(documentData);
            var uploadResult = await _documentsRepository.SaveDocumentMetadataForProjectAsync(projectId, creationDto);

            return FromRepositoryResult<DocumentDto, DocumentGet>(uploadResult, _mapper);
        }

        [HttpPost("{documentId}/content")]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(DocumentGet), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UploadDocumentContentAsync(Guid projectId,
            Guid documentId,
            [Required] IFormFile document)
        {
            if (!await _projectsRepository.CheckIfProjectExistsAsync(projectId))
            {
                return NotFound();
            }

            var contentSaveResult = await _documentsRepository.SaveDocumentContentAsync(projectId, documentId, document);

            return FromRepositoryResult<DocumentDto, DocumentGet>(contentSaveResult, _mapper);
        }
    }
}
