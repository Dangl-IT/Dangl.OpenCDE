using Dangl.Data.Shared;
using Dangl.OpenCDE.Data.Dto.Documents;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.Repository
{
    public interface IDocumentsRepository
    {
        IQueryable<DocumentDto> GetAllDocumentsForProject(Guid projectId, string filter);

        Task<RepositoryResult<DocumentDto>> GetDocumentByIdAsync(Guid documentId);

        Task<RepositoryResult<DocumentDto>> SaveDocumentMetadataForProjectAsync(Guid projectId, CreateDocumentDto metadata);

        Task<RepositoryResult<DocumentDto>> SaveDocumentContentAsync(Guid projectId, Guid documentId, IFormFile document);

        Task<RepositoryResult<DocumentFileResultDto>> GetDocumentDataAsync(Guid documentId);
    }
}
