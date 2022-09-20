using Dangl.Data.Shared;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Data.Dto.OpenCdeDocumentSelection;
using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.Repository
{
    public interface IOpenCdeDocumentSelectionRepository
    {
        Task<RepositoryResult<(Guid userId, TokenStorageDto tokenStorage)>> GetUserSessionDataForDocumentDownloadAsync(Guid documentSessionId);
        
        Task<RepositoryResult<(Guid userId, TokenStorageDto tokenStorage)>> GetUserSessionDataForDocumentUploadAsync(Guid documentSessionId);

        Task<RepositoryResult<DocumentsToUpload>> GetUploadInstructionsAsync(Guid documentSessionId, UploadFileDetails uploadFileDetails);

        Task<RepositoryResult> MarkCdeSessionFileUploadAsCancelledAsync(Guid documentSessionId, string sessionFileId);
        
        Task<RepositoryResult<DocumentDto>> MarkCdeSessionFileUploadAsFinishedAsync(Guid documentSessionId, string sessionFileId);

        Task<RepositoryResult<(Guid sessionId, int validForSeconds)>> PrepareOpenCdeDocumentDownloadSelectionAsync(string clientCallbackUrl,
            string userJwt,
            long userJwtExpiresAt);

        Task<RepositoryResult<(Guid sessionId, int validForSeconds)>> PrepareOpenCdeDocumentUploadSelectionAsync(string clientCallbackUrl,
            string userJwt,
            long userJwtExpiresAt,
            List<FileToUpload> filesToUpload);

        Task<RepositoryResult<DocumentSelectionFinalizationDto>> FinalizeOpenCdeDocumentDownloadAsync(Guid documentSessionId,
            Guid documentId);

        Task<RepositoryResult<DocumentDto>> GetDocumentForSelectionAsync(Guid selectionId);

        /// <summary>
        /// This returns the callback url for the client session
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="documentUploadSessionId"></param>
        /// <returns></returns>
        Task<RepositoryResult<string>> SelectProjectForDocumentUploadSession(Guid projectId, Guid documentUploadSessionId);
    }
}
