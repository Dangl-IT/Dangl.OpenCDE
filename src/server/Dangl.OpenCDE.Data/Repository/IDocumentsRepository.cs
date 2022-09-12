using Dangl.Data.Shared;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Shared.Models.Controllers.Documents;
using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.Repository
{
    public interface IDocumentsRepository
    {
        IQueryable<DocumentDto> GetAllDocumentsForProject(Guid projectId, string filter);

        IQueryable<DocumentDto> GetAllDocumentsById(List<Guid> documentIds);
        
        Task<RepositoryResult<DocumentDto>> GetDocumentByIdAsync(Guid documentId);

        Task<RepositoryResult<DocumentDto>> SaveDocumentMetadataForProjectAsync(Guid projectId, CreateDocumentDto metadata);

        Task<RepositoryResult<DocumentDto>> SaveDocumentContentAsync(Guid projectId, Guid documentId, IFormFile document);

        Task<RepositoryResult<DocumentFileResultDto>> GetDocumentDataAsync(Guid documentId);

        Task<bool> CheckIfDocumentWithoutContentExistsForProject(Guid projectId, Guid documentId);

        Task<RepositoryResult<DocumentContentSasUploadResultGet>> PrepareSasDocumentUploadAsync(Guid documentId, string fileName, string mimeType, long sizeInBytes);

        Task<RepositoryResult> MarkDocumentAsUploadedAsync(Guid documentId);
    }
}
