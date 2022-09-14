using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dangl.AspNetCore.FileHandling;
using Dangl.AspNetCore.FileHandling.Azure;
using Dangl.Data.Shared;
using Dangl.Identity.Client.Mvc.Services;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Data.Dto.OpenCdeDocumentSelection;
using Dangl.OpenCDE.Data.Models;
using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.Repository
{
    public class OpenCdeDocumentSelectionRepository : IOpenCdeDocumentSelectionRepository
    {
        private readonly CdeDbContext _context;
        private readonly IUserInfoService _userInfoService;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;
        private readonly IDocumentsRepository _documentsRepository;
        public const long FILE_UPLOAD_MAX_SIZE_IN_BYTES = 1_000_000_000; // Just setting to 1 GB for the demo

        public OpenCdeDocumentSelectionRepository(CdeDbContext context,
            IUserInfoService userInfoService,
            IMapper mapper,
            IFileManager fileManager,
            IDocumentsRepository documentsRepository)
        {
            _context = context;
            _userInfoService = userInfoService;
            _mapper = mapper;
            _fileManager = fileManager;
            _documentsRepository = documentsRepository;
        }

        public async Task<RepositoryResult<(Guid userId, TokenStorageDto tokenStorage)>> GetUserSessionDataForDocumentDownloadAsync(Guid documentSessionId)
        {
            var session = await _context
                .OpenCdeDocumentDownloadSessions
                .FirstOrDefaultAsync(s => s.Id == documentSessionId
                    && s.ValidUntilUtc >= DateTimeOffset.UtcNow);
            if (session == null)
            {
                return RepositoryResult<(Guid userId, TokenStorageDto tokenStorage)>.Fail("No valid session found for the given id.");
            }

            var tokenStorage = JsonConvert.DeserializeObject<TokenStorageDto>(session.AuthenticationInformationJson);

            return RepositoryResult<(Guid userId, TokenStorageDto tokenStorage)>.Success((session.UserId, tokenStorage));
        }

        public async Task<RepositoryResult<(Guid sessionId, int validForSeconds)>> PrepareOpenCdeDocumentDownloadSelectionAsync(string clientCallbackUrl,
            string userJwt,
            long userJwtExpiresAt)
        {
            if (string.IsNullOrWhiteSpace(clientCallbackUrl))
            {
                return RepositoryResult<(Guid sessionId, int validForSeconds)>.Fail("The client callback url can not be empty.");
            }

            if (!await _userInfoService.UserIsAuthenticatedAsync())
            {
                return RepositoryResult<(Guid sessionId, int validForSeconds)>.Fail("There is no user context present in the current request.");
            }

            // This session can be initiated within the next 5 minutes
            var sessionValidityInSeconds = 300;

            var session = new OpenCdeDocumentDownloadSession
            {
                UserId = await _userInfoService.GetCurrentUserIdAsync(),
                ValidUntilUtc = DateTimeOffset.UtcNow.AddSeconds(sessionValidityInSeconds),
                ClientCallbackUrl = clientCallbackUrl,
                AuthenticationInformationJson = JsonConvert.SerializeObject(new TokenStorageDto
                {
                    ExpiresAt = userJwtExpiresAt,
                    JsonWebToken = userJwt
                })
            };

            _context.OpenCdeDocumentDownloadSessions.Add(session);
            await _context.SaveChangesAsync();

            return RepositoryResult<(Guid sessionId, int validForSeconds)>.Success((session.Id, sessionValidityInSeconds));
        }

        public async Task<RepositoryResult<(Guid sessionId, int validForSeconds)>> PrepareOpenCdeDocumentUploadSelectionAsync(
            string clientCallbackUrl,
            string userJwt,
            long userJwtExpiresAt,
            List<FileToUpload> filesToUpload)
        {
            if (string.IsNullOrWhiteSpace(clientCallbackUrl))
            {
                return RepositoryResult<(Guid sessionId, int validForSeconds)>.Fail("The client callback url can not be empty.");
            }

            if (!await _userInfoService.UserIsAuthenticatedAsync())
            {
                return RepositoryResult<(Guid sessionId, int validForSeconds)>.Fail("There is no user context present in the current request.");
            }

            if (!(filesToUpload?.Any() ?? false))
            {
                return RepositoryResult<(Guid sessionId, int validForSeconds)>.Fail("No files to upload were provided.");
            }

            if (filesToUpload.Any(f => string.IsNullOrWhiteSpace(f.FileName)))
            {
                return RepositoryResult<(Guid sessionId, int validForSeconds)>.Fail("One or more files to upload have no file name.");
            }

            if (filesToUpload.Count != filesToUpload.Select(f => f.SessionFileId).Distinct().Count())
            {
                return RepositoryResult<(Guid sessionId, int validForSeconds)>.Fail("The session file ids must be unique.");
            }
                
            // This session can be initiated within the next 5 minutes
            var sessionValidityInSeconds = 300;

            var session = new OpenCdeDocumentUploadSession
            {
                UserId = await _userInfoService.GetCurrentUserIdAsync(),
                ValidUntilUtc = DateTimeOffset.UtcNow.AddSeconds(sessionValidityInSeconds),
                ClientCallbackUrl = clientCallbackUrl,
                AuthenticationInformationJson = JsonConvert.SerializeObject(new TokenStorageDto
                {
                    ExpiresAt = userJwtExpiresAt,
                    JsonWebToken = userJwt
                }),
                PendingFiles = filesToUpload
                    .Select(f => new PendingOpenCdeUploadFile
                    {
                        FileName = f.FileName,
                        SessionFileId = f.SessionFileId
                    })
                    .ToList()
            };

            _context.OpenCdeDocumentUploadSessions.Add(session);
            await _context.SaveChangesAsync();

            return RepositoryResult<(Guid sessionId, int validForSeconds)>.Success((session.Id, sessionValidityInSeconds));
        }

        public async Task<RepositoryResult<DocumentSelectionFinalizationDto>> FinalizeOpenCdeDocumentDownloadAsync(Guid documentSessionId,
            Guid documentId)
        {
            var currentUserId = await _userInfoService
                .GetCurrentUserIdAsync();

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var dbSession = await _context
                .OpenCdeDocumentDownloadSessions
                .FirstOrDefaultAsync(s => s.Id == documentSessionId
                    && s.UserId == currentUserId);
            if (dbSession == null)
            {
                return RepositoryResult<DocumentSelectionFinalizationDto>.Fail("There is no session with the given id for the current user.");
            }

            _context.OpenCdeDocumentDownloadSessions.Remove(dbSession);
            await _context.SaveChangesAsync();

            var documentExists = await _context.Documents.AnyAsync(d => d.Id == documentId
                && d.FileId != null);
            if (!documentExists)
            {
                return RepositoryResult<DocumentSelectionFinalizationDto>.Fail("Invalid document id, document was either not found or has no content.");
            }

            var documentSelection = new OpenCdeDocumentSelection
            {
                DocumentId = documentId,
                UserId = currentUserId
            };
            _context.OpenCdeDocumentSelections.Add(documentSelection);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return RepositoryResult<DocumentSelectionFinalizationDto>.Success(new DocumentSelectionFinalizationDto
            {
                ClientCallbackUrl = dbSession.ClientCallbackUrl,
                SelectionId = documentSelection.Id
            });
        }

        public async Task<RepositoryResult<DocumentDto>> GetDocumentForSelectionAsync(Guid selectionId)
        {
            var userId = await _userInfoService.GetCurrentUserIdAsync();
            var document = await _context
                .OpenCdeDocumentSelections
                .Where(s => s.Id == selectionId && s.UserId == userId)
                .Select(d => d.Document)
                .ProjectTo<DocumentDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (document == null)
            {
                return RepositoryResult<DocumentDto>.Fail("There is no selection with the given id for the current user.");
            }

            return RepositoryResult<DocumentDto>.Success(document);
        }

        public async Task<RepositoryResult<DocumentsToUpload>> GetUploadInstructionsAsync(Guid documentSessionId,
            UploadFileDetails uploadFileDetails)
        {
            var azureBlobManager = _fileManager as AzureBlobFileManager;
            if (azureBlobManager == null)
            {
                return RepositoryResult<DocumentsToUpload>.Fail("There is no Azure Blob connection specified, " +
                    "the server is using a type of storage which does not have an implementation for the upload flow.");
            }

            var dbUploadSession = await _context
                .OpenCdeDocumentUploadSessions
                .Include(s => s.PendingFiles)
                .FirstOrDefaultAsync(us => us.Id == documentSessionId);
            if (dbUploadSession == null)
            {
                return RepositoryResult<DocumentsToUpload>.Fail("Could not find the upload session.");
            }

            if (dbUploadSession.SelectedProjectId == null)
            {
                return RepositoryResult<DocumentsToUpload>.Fail("This document upload session has not yet a project assigned.");
            }

            var uploadInstructions = new DocumentsToUpload();

            uploadInstructions.ServerContext = documentSessionId.ToString();

            uploadInstructions._DocumentsToUpload ??= new List<DocumentToUpload>();
            foreach (var file in uploadFileDetails.Files)
            {
                if (file.SizeInBytes > FILE_UPLOAD_MAX_SIZE_IN_BYTES)
                {
                    return RepositoryResult<DocumentsToUpload>.Fail("One file exceeds the max upload size.");
                }

                if (file.SizeInBytes <= 0)
                {
                    return RepositoryResult<DocumentsToUpload>.Fail("File sizes can not be zero or negative.");
                }

                var pendingFile = dbUploadSession.PendingFiles.FirstOrDefault(f => f.SessionFileId == file.SessionFileId);
                if (pendingFile == null)
                {
                    return RepositoryResult<DocumentsToUpload>.Fail("A file was found that was not previously announced.");
                }

                var documentResult = await _documentsRepository.SaveDocumentMetadataForProjectAsync(dbUploadSession.SelectedProjectId.Value,
                    new CreateDocumentDto
                    {
                        Description = "This document was uploaded via the Open CDE Documents API",
                        Name = pendingFile.FileName
                    });

                if (!documentResult.IsSuccess)
                {
                    return RepositoryResult<DocumentsToUpload>.Fail(documentResult.ErrorMessage);
                }

                pendingFile.LinkedCdeDocumentId = documentResult.Value.Id;
                await _context.SaveChangesAsync();

                var uploadData = await _documentsRepository.PrepareSasDocumentUploadAsync(documentResult.Value.Id,
                    pendingFile.FileName,
                    "application/octet-stream",
                    file.SizeInBytes);
                if (!uploadData.IsSuccess)
                {
                    return RepositoryResult<DocumentsToUpload>.Fail(uploadData.ErrorMessage);
                }

                uploadInstructions._DocumentsToUpload.Add(new DocumentToUpload
                {
                    SessionFileId = file.SessionFileId,
                    UploadFileParts = new List<UploadFilePartInstruction>
                    {
                        new UploadFilePartInstruction
                        {
                            ContentRangeStart = 0,
                            ContentRangeEnd = file.SizeInBytes - 1,
                            HttpMethod = UploadFilePartInstruction.HttpMethodEnum.PUTEnum,
                            Url = uploadData.Value.SasUploadLink.UploadLink,
                            IncludeAuthorization = false,
                            AdditionalHeaders = new Headers
                            {
                                Values = uploadData.Value.CustomHeaders
                                    .Select(ch => new HeaderValue
                                        {
                                            Name = ch.Name,
                                            Value = ch.Value
                                        })
                                    .ToList()
                            }
                        }
                    }
                });
            }

            return RepositoryResult<DocumentsToUpload>.Success(uploadInstructions);
        }

        public async Task<RepositoryResult<string>> SelectProjectForDocumentUploadSession(Guid projectId, Guid documentUploadSessionId)
        {
            var projectExists = await _context
                .Projects
                .AnyAsync(p => p.Id == projectId);
            if (!projectExists)
            {
                return RepositoryResult<string>.Fail("There is no project with the given id.");
            }

            var dbSession = await _context
                .OpenCdeDocumentUploadSessions
                .FirstOrDefaultAsync(s => s.Id == documentUploadSessionId);
            if (dbSession == null)
            {
                return RepositoryResult<string>.Fail("There is no session with the given id.");

            }

            if (dbSession.SelectedProjectId != null)
            {
                return RepositoryResult<string>.Fail("The session has already been assigned to a project.");

            }

            dbSession.SelectedProjectId = projectId;
            await _context.SaveChangesAsync();
            return RepositoryResult<string>.Success(dbSession.ClientCallbackUrl);
        }

        public async Task<RepositoryResult> MarkCdeSessionFileUploadAsCancelledAsync(Guid documentSessionId, string sessionFileId)
        {
            var pendingFile = await _context
                .PendingOpenCdeUploadFiles
                .Include(pf => pf.UploadSession)
                .FirstOrDefaultAsync(pf => pf.UploadSessionId == documentSessionId
                    && pf.SessionFileId == sessionFileId);
            if (pendingFile == null)
            {
                return RepositoryResult.Fail("There is no pending file with the given ids.");
            }

            pendingFile.IsCancelled = true;
            
            if (pendingFile.LinkedCdeDocumentId != null
                && pendingFile.UploadSession.SelectedProjectId != null)
            {
                await _documentsRepository.DeleteDocumentAsync(pendingFile.UploadSession.SelectedProjectId.Value,
                    pendingFile.LinkedCdeDocumentId.Value);
            }

            await _context.SaveChangesAsync();

            return RepositoryResult.Success();

            throw new NotImplementedException();
        }

        public async Task<RepositoryResult<DocumentDto>> MarkCdeSessionFileUploadAsFinishedAsync(Guid documentSessionId, string sessionFileId)
        {
            var uploadSession = await _context
                .OpenCdeDocumentUploadSessions
                .Include(s => s.PendingFiles)
                .Where(s => s.Id == documentSessionId)
                .FirstOrDefaultAsync();
            if (uploadSession == null)
            {
                return RepositoryResult<DocumentDto>.Fail("There is no active upload session with the given id.");
            }

            var pendingFile = uploadSession
                .PendingFiles
                .FirstOrDefault(pf => pf.SessionFileId == sessionFileId);
            if (pendingFile == null)
            {
                return RepositoryResult<DocumentDto>.Fail("There is no pending file with the given ids.");
            }

            if (pendingFile.LinkedCdeDocumentId == null)
            {
                return RepositoryResult<DocumentDto>.Fail("The upload for the current file has not been initalized.");
            }

            var documentSaveResult = await _documentsRepository.MarkDocumentAsUploadedAsync(pendingFile.LinkedCdeDocumentId.Value);
            if (!documentSaveResult.IsSuccess)
            {
                return RepositoryResult<DocumentDto>.Fail(documentSaveResult.ErrorMessage);
            }

            var documentDataResult = await _documentsRepository.GetDocumentByIdAsync(pendingFile.LinkedCdeDocumentId.Value);
            if (!documentDataResult.IsSuccess)
            {
                return RepositoryResult<DocumentDto>.Fail(documentDataResult.ErrorMessage);
            }

            if (uploadSession.PendingFiles.Count == 1)
            {
                // If we're marking the last file, then we can remove the session completely
                _context.OpenCdeDocumentUploadSessions.Remove(uploadSession);
            }

            _context.PendingOpenCdeUploadFiles.Remove(pendingFile);
            await _context.SaveChangesAsync();

            return RepositoryResult<DocumentDto>.Success(documentDataResult.Value);
        }
    }
}
