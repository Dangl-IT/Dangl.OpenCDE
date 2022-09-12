using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dangl.AspNetCore.FileHandling.Azure;
using Dangl.Data.Shared;
using Dangl.Data.Shared.QueryUtilities;
using Dangl.Identity.Client.Mvc.Services;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Data.IO;
using Dangl.OpenCDE.Data.Models;
using Dangl.OpenCDE.Shared.Models.Controllers.Documents;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.Repository
{
    public class DocumentsRepository : IDocumentsRepository
    {
        private readonly CdeDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICdeAppFileHandler _cdeAppFileHandler;
        private readonly IUserInfoService _userInfoService;

        public DocumentsRepository(CdeDbContext context,
            IMapper mapper,
            ICdeAppFileHandler cdeAppFileHandler,
            IUserInfoService userInfoService)
        {
            _context = context;
            _mapper = mapper;
            _cdeAppFileHandler = cdeAppFileHandler;
            _userInfoService = userInfoService;
        }

        public Task<bool> CheckIfDocumentWithoutContentExistsForProject(Guid projectId, Guid documentId)
        {
            return _context
                .Documents
                .AnyAsync(d => d.ProjectId == projectId
                    && d.Id == documentId
                    && (d.FileId == null || !d.File.FileAvailableInStorage));
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

        public IQueryable<DocumentDto> GetAllDocumentsById(List<Guid> documentIds)
        {
            return _context
                    .Documents
                    .Where(d => documentIds.Contains(d.Id))
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

        public async Task<RepositoryResult> MarkDocumentAsUploadedAsync(Guid documentId)
        {
            var document = await _context
                .Documents
                .Include(d => d.File)
                .FirstOrDefaultAsync(d => d.Id == documentId);
            if (document?.File == null)
            {
                return RepositoryResult.Fail("There is either no document with the given id, or it has no file reference.");
            }

            if (document.File.FileAvailableInStorage)
            {
                return RepositoryResult.Fail("The file is already marked as available.");
            }

            if (!await _cdeAppFileHandler.CheckIfFileExistsInStorageAsync(document.FileId.Value))
            {
                return RepositoryResult.Fail("The file does not seem to exist in the storage.");
            }

            document.File.FileAvailableInStorage = true;
            await _context.SaveChangesAsync();

            return RepositoryResult.Success();
        }

        public async Task<RepositoryResult<DocumentContentSasUploadResultGet>> PrepareSasDocumentUploadAsync(Guid documentId,
            string fileName,
            string mimeType,
            long sizeInBytes)
        {
            var document = await _context
                .Documents
                .Include(d => d.File)
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null)
            {
                return RepositoryResult<DocumentContentSasUploadResultGet>.Fail("There is no document with the given id present in the project.");
            }

            if (document.FileId != null)
            {
                return RepositoryResult<DocumentContentSasUploadResultGet>.Fail("This document already has content, it can not be changed.");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return RepositoryResult<DocumentContentSasUploadResultGet>.Fail("The file name is required.");
            }

            if (string.IsNullOrWhiteSpace(mimeType))
            {
                return RepositoryResult<DocumentContentSasUploadResultGet>.Fail("The content type is required.");
            }

            var userId = await _userInfoService.GetCurrentUserIdAsync();
            var dbMimeType = await _cdeAppFileHandler.GetDbMimeTypeAsync(mimeType);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var dbFile = new CdeAppFile
            {
                ContainerName = FileContainerNames.PROJECT_DOCUMENTS,
                FileName = fileName,
                MimeType = dbMimeType,
                FileAvailableInStorage = false,
                CreatedByUserId = userId,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                SizeInBytes = sizeInBytes
            };

            document.File = dbFile;

            await _context.SaveChangesAsync();

            var sasUploadLinkResult = await _cdeAppFileHandler.TryGetSasUploadLinkAsync(dbFile.Id);
            if (!sasUploadLinkResult.IsSuccess)
            {
                return RepositoryResult<DocumentContentSasUploadResultGet>.Fail(sasUploadLinkResult.ErrorMessage);
            }

            await transaction.CommitAsync();

            var uploadLinkData = new DocumentContentSasUploadResultGet
            {
                SasUploadLink = sasUploadLinkResult.Value,
                CustomHeaders = new System.Collections.Generic.List<DocumentContentSasUploadResultHeaderGet>
                {
                    new DocumentContentSasUploadResultHeaderGet
                    {
                        Name = "x-ms-blob-type",
                        Value = "BlockBlob"
                    }
                }
            };

            return RepositoryResult<DocumentContentSasUploadResultGet>.Success(uploadLinkData);
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
