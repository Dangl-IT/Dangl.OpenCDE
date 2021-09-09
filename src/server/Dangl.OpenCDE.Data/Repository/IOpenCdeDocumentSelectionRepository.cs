using Dangl.Data.Shared;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Data.Dto.OpenCdeDocumentSelection;
using System;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.Repository
{
    public interface IOpenCdeDocumentSelectionRepository
    {
        Task<RepositoryResult<(Guid userId, TokenStorageDto tokenStorage)>> GetUserSessionDataForDocumentSessionAsync(Guid documentSessionId);

        Task<RepositoryResult<(Guid sessionId, int validForSeconds)>> PrepareOpenCdeDocumentSelectionAsync(string clientCallbackUrl,
            string userJwt,
            long userJwtExpiresAt);

        Task<RepositoryResult<DocumentSelectionFinalizationDto>> FinalizeOpenCdeDocumentSelectionAsync(Guid documentSessionId,
            Guid documentId);

        Task<RepositoryResult<DocumentDto>> GetDocumentForSelectionAsync(Guid selectionId);
    }
}
