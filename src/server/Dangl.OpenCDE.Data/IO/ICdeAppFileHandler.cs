using Dangl.AspNetCore.FileHandling.Azure;
using Dangl.Data.Shared;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.IO
{
    public interface ICdeAppFileHandler
    {
        /// <summary>
        /// This will also return a successful result even if the file could not be persisted to storage, only
        /// the metadata. It will only return a failure if an exception itself happens that was not caught inside
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <param name="container"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        Task<RepositoryResult<Guid>> SaveFileAsync(Stream fileStream, string fileName, string container, string mimeType);

        Task<RepositoryResult<FileResultContainer>> GetFileByIdAsync(Guid fileId);

        Task<RepositoryResult<SasDownloadLink>> TryGetFileSasDownloadLinkAsync(Guid fileId);

        Task<RepositoryResult> DeleteFileAsync(Guid fileId);
    }
}
