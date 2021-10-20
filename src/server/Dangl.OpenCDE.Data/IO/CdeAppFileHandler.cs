using Dangl.AspNetCore.FileHandling;
using Dangl.AspNetCore.FileHandling.Azure;
using Dangl.Data.Shared;
using Dangl.Identity.Client.Mvc.Services;
using Dangl.OpenCDE.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.IO
{
    public class CdeAppFileHandler : ICdeAppFileHandler
    {
        public const string DEFAULT_MIME_TYPE = "application/octet-stream";

        private readonly CdeDbContext _context;
        private readonly IFileManager _fileManager;
        private readonly ILogger _logger;
        private readonly IUserInfoService _userInfoService;

        public CdeAppFileHandler(CdeDbContext context,
            IFileManager fileManager,
            ILoggerFactory loggerFactory,
            IUserInfoService userInfoService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            _logger = loggerFactory?.CreateLogger<CdeAppFileHandler>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        }

        public async Task<RepositoryResult<FileResultContainer>> GetFileByIdAsync(Guid fileId)
        {
            var dbFile = await _context.Files
                .Include(f => f.MimeType)
                .FirstOrDefaultAsync(f => f.Id == fileId);

            if (dbFile == null)
            {
                return RepositoryResult<FileResultContainer>.Fail("There is no file with the given id");
            }

            var fileResult = await _fileManager.GetFileAsync(fileId, dbFile.ContainerName, dbFile.FileName);

            if (!fileResult.IsSuccess)
            {
                return RepositoryResult<FileResultContainer>.Fail("The file can not be accessed");
            }

            return RepositoryResult<FileResultContainer>.Success(new FileResultContainer(fileResult.Value,
                dbFile.FileName,
                dbFile.MimeType.MimeType));
        }

        public async Task<RepositoryResult<Guid>> SaveFileAsync(Stream fileStream,
            string fileName,
            string container,
            string mimeType)
        {
            fileName = NormalizeFilename(fileName);
            var dbMimeType = await GetDbMimeTypeAsync(mimeType);

            Guid? userId = null;
            if (await _userInfoService.UserIsAuthenticatedAsync())
            {
                userId = await _userInfoService.GetCurrentUserIdAsync();
            }

            var dbFile = new CdeAppFile
            {
                ContainerName = container,
                FileName = fileName.WithMaxLength(255),
                MimeType = dbMimeType,
                SizeInBytes = fileStream.Length,
                CreatedByUserId = userId
            };
            _context.Files.Add(dbFile);
            await _context.SaveChangesAsync();
            try
            {
                var fileSaveResult = await _fileManager.SaveFileAsync(dbFile.Id, container, fileName, fileStream);
                if (fileSaveResult.IsSuccess)
                {
                    return RepositoryResult<Guid>.Success(dbFile.Id);
                }

                return RepositoryResult<Guid>.Fail(fileSaveResult.ErrorMessage);
            }
            catch (Exception fileHandlerException)
            {
                _logger.LogError(fileHandlerException, "Error during file save");
                await _context.SaveChangesAsync();
                return RepositoryResult<Guid>.Fail($"Internal error in {nameof(_fileManager)}:{Environment.NewLine}{fileHandlerException}");
            }
        }

        private static string NormalizeFilename(string originalFilename)
        {
            if (string.IsNullOrWhiteSpace(originalFilename))
            {
                return string.Empty;
            }
            return Path.GetFileName(originalFilename).Trim();
        }

        public async Task<CdeAppFileMimeType> GetDbMimeTypeAsync(string mimeType)
        {
            mimeType = mimeType?.ToLowerInvariant().Trim();
            if (string.IsNullOrWhiteSpace(mimeType))
            {
                mimeType = DEFAULT_MIME_TYPE;
            }

            return await _context.FileMimeTypes
#pragma warning disable RCS1155 // Use StringComparison when comparing strings. -> Because of Linq to SQL translation via EF Core
                                 .FirstOrDefaultAsync(m => m.MimeType.ToUpper() == mimeType.ToUpper())
#pragma warning restore RCS1155 // Use StringComparison when comparing strings.
                             ?? new CdeAppFileMimeType { MimeType = mimeType };
        }

        public async Task<RepositoryResult> DeleteFileAsync(Guid fileId)
        {
            var dbFile = await _context.Files
                .FirstOrDefaultAsync(f => f.Id == fileId);
            if (dbFile == null)
            {
                return RepositoryResult.Fail("There is no file with the given id");
            }

            var deletionResult = await _fileManager.DeleteFileAsync(dbFile.Id, dbFile.ContainerName, dbFile.FileName);
            if (deletionResult.IsSuccess)
            {
                _context.Files.Remove(dbFile);
                await _context.SaveChangesAsync();
            }
            return deletionResult;
        }

        public async Task<RepositoryResult<SasDownloadLink>> TryGetFileSasDownloadLinkAsync(Guid fileId)
        {
            if (!(_fileManager is AzureBlobFileManager azureBlobFileManager))
            {
                return RepositoryResult<SasDownloadLink>.Fail("SAS links can only be generated for Azure Blob Storage");
            }

            var dbFile = await _context.Files
                .Include(f => f.MimeType)
                .FirstOrDefaultAsync(f => f.Id == fileId);

            if (dbFile == null)
            {
                return RepositoryResult<SasDownloadLink>.Fail("There is no file with the given id");
            }

            var sasDownloadLinkResult = await azureBlobFileManager
                .GetSasDownloadLinkAsync(fileId,
                    dbFile.ContainerName,
                    dbFile.FileName,
                    friendlyFileName: dbFile.FileName);

            return sasDownloadLinkResult;
        }

        public async Task<RepositoryResult<SasUploadLink>> TryGetSasUploadLinkAsync(Guid fileId)
        {
            if (!(_fileManager is AzureBlobFileManager azureBlobFileManager))
            {
                return RepositoryResult<SasUploadLink>.Fail("SAS links can only be generated for Azure Blob Storage");
            }

            var dbFile = await _context.Files
                .Include(f => f.MimeType)
                .FirstOrDefaultAsync(f => f.Id == fileId);

            if (dbFile == null)
            {
                return RepositoryResult<SasUploadLink>.Fail("There is no file with the given id");
            }

            if (dbFile.FileAvailableInStorage)
            {
                return RepositoryResult<SasUploadLink>.Fail("The file has already been uploaded.");
            }

            var sasUploadLinkResult = await azureBlobFileManager
                .GetSasUploadLinkAsync(fileId,
                dbFile.ContainerName,
                dbFile.FileName,
                validForMinutes: 5);

            return sasUploadLinkResult;
        }

        public async Task<bool> CheckIfFileExistsInStorageAsync(Guid fileId)
        {
            var dbFile = await _context
                .Files
                .FirstOrDefaultAsync(f => f.Id == fileId);
            if (dbFile == null)
            {
                return false;
            }

            var fileExistsResult = await _fileManager.CheckIfFileExistsAsync(dbFile.Id, dbFile.ContainerName, dbFile.FileName);
            if (!fileExistsResult.IsSuccess)
            {
                return false;
            }

            return fileExistsResult.Value;
        }
    }
}
