using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dangl.Data.Shared;
using Dangl.Data.Shared.QueryUtilities;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Data.IO;
using Dangl.OpenCDE.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.Repository
{
    public class DocumentsRepository : IDocumentsRepository
    {
        private readonly CdeDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICdeAppFileHandler _cdeAppFileHandler;

        public DocumentsRepository(CdeDbContext context,
            IMapper mapper,
            ICdeAppFileHandler cdeAppFileHandler)
        {
            _context = context;
            _mapper = mapper;
            _cdeAppFileHandler = cdeAppFileHandler;
        }

        public IQueryable<DocumentDto> GetAllDocumentsForProject(Guid projectId, string filter)
        {
            return _context
                .Documents
                .Where(d => d.ProjectId == projectId)
                .Filter(filter, text => document => EF.Functions.Like(document.Name, $"%{text}%")
                    || EF.Functions.Like(document.Description, $"%{text}%"),
                    transformFilterToLowercase: true)
                .ProjectTo<DocumentDto>(_mapper.ConfigurationProvider);
        }

        public async Task<RepositoryResult<DocumentDto>> GetDocumentByIdAsync(Guid documentId)
        {
            var document = await _context
                .Documents
                .ProjectTo<DocumentDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(d => d.Id == documentId
                    && d.ContentAvailable);
            if (document == null)
            {
                return RepositoryResult<DocumentDto>.Fail("There is no document with the given id present, or it has no content available.");
            }

            return RepositoryResult<DocumentDto>.Success(document);
        }

        public async Task<RepositoryResult<DocumentFileResultDto>> GetDocumentDataAsync(Guid documentId)
        {
            var document = await _context
                .Documents
                .Where(d => d.Id == documentId)
                .Select(d => new { d.FileId })
                .FirstOrDefaultAsync();
            if (document == null)
            {
                return RepositoryResult<DocumentFileResultDto>.Fail("There is no document with the given id present.");
            }

            if (document.FileId == null)
            {
                return RepositoryResult<DocumentFileResultDto>.Fail("There is no content available for this document.");
            }

            var sasDownloadLinkResult = await _cdeAppFileHandler.TryGetFileSasDownloadLinkAsync(document.FileId.Value);
            if (sasDownloadLinkResult.IsSuccess)
            {
                return RepositoryResult<DocumentFileResultDto>.Success(new DocumentFileResultDto
                {
                    SasDownloadLink = sasDownloadLinkResult.Value
                });
            }

            var fileResult = await _cdeAppFileHandler.GetFileByIdAsync(document.FileId.Value);
            if (!fileResult.IsSuccess)
            {
                return RepositoryResult<DocumentFileResultDto>.Fail(fileResult.ErrorMessage);
            }

            return RepositoryResult<DocumentFileResultDto>.Success(new DocumentFileResultDto
            {
                FileResultContainer = fileResult.Value
            });
        }

        public async Task<RepositoryResult<DocumentDto>> SaveDocumentContentAsync(Guid projectId, Guid documentId, IFormFile document)
        {
            var dbDocument = await _context
                .Documents
                .Where(d => d.ProjectId == projectId && d.Id == documentId)
                .FirstOrDefaultAsync();
            if (dbDocument == null)
            {
                return RepositoryResult<DocumentDto>.Fail("There is no document with the given id present in the project.");
            }

            if (dbDocument.FileId != null)
            {
                return RepositoryResult<DocumentDto>.Fail("This document already has content, it can not be changed.");
            }

            await using var fileStream = document.OpenReadStream();

            var fileName = document.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "document.bin";
            }

            var mimeType = document.ContentType;
            if (string.IsNullOrWhiteSpace(mimeType))
            {
                mimeType = "application/octet-stream";
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var fileSaveResult = await _cdeAppFileHandler.SaveFileAsync(fileStream,
                fileName,
                FileContainerNames.PROJECT_DOCUMENTS,
                mimeType);

            if (!fileSaveResult.IsSuccess)
            {
                return RepositoryResult<DocumentDto>.Fail(fileSaveResult.ErrorMessage);
            }

            dbDocument.FileId = fileSaveResult.Value;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // We need to have loaded the file data for later mapping into the Dto
            await _context.Files.Where(f => f.Id == fileSaveResult.Value).LoadAsync();

            return RepositoryResult<DocumentDto>.Success(_mapper.Map<DocumentDto>(dbDocument));
        }

        public async Task<RepositoryResult<DocumentDto>> SaveDocumentMetadataForProjectAsync(Guid projectId, CreateDocumentDto metadata)
        {
            if (string.IsNullOrWhiteSpace(metadata?.Name))
            {
                return RepositoryResult<DocumentDto>.Fail("The document name is required.");
            }

            if (!(await _context.Projects.AnyAsync(p => p.Id == projectId)))
            {
                return RepositoryResult<DocumentDto>.Fail("There is no document with the given id.");
            }

            var dbDocument = new Document
            {
                Description = metadata.Description,
                Name = metadata.Name,
                ProjectId = projectId
            };
            _context.Documents.Add(dbDocument);
            await _context.SaveChangesAsync();

            return RepositoryResult<DocumentDto>.Success(_mapper.Map<DocumentDto>(dbDocument));
        }
    }
}
