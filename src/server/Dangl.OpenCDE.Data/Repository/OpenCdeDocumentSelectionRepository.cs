using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dangl.Data.Shared;
using Dangl.Identity.Client.Mvc.Services;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Data.Dto.OpenCdeDocumentSelection;
using Dangl.OpenCDE.Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.Repository
{
    public class OpenCdeDocumentSelectionRepository : IOpenCdeDocumentSelectionRepository
    {
        private readonly CdeDbContext _context;
        private readonly IUserInfoService _userInfoService;
        private readonly IMapper _mapper;

        public OpenCdeDocumentSelectionRepository(CdeDbContext context,
            IUserInfoService userInfoService,
            IMapper mapper)
        {
            _context = context;
            _userInfoService = userInfoService;
            _mapper = mapper;
        }

        public async Task<RepositoryResult<(Guid userId, TokenStorageDto tokenStorage)>> GetUserSessionDataForDocumentSessionAsync(Guid documentSessionId)
        {
            var session = await _context
                .OpenCdeDocumentSelectionSessions
                .FirstOrDefaultAsync(s => s.Id == documentSessionId
                    && s.ValidUntilUtc >= DateTimeOffset.UtcNow);
            if (session == null)
            {
                return RepositoryResult<(Guid userId, TokenStorageDto tokenStorage)>.Fail("No valid session found for the given id.");
            }

            var tokenStorage = JsonConvert.DeserializeObject<TokenStorageDto>(session.AuthenticationInformationJson);

            return RepositoryResult<(Guid userId, TokenStorageDto tokenStorage)>.Success((session.UserId, tokenStorage));
        }

        public async Task<RepositoryResult<(Guid sessionId, int validForSeconds)>> PrepareOpenCdeDocumentSelectionAsync(string clientCallbackUrl,
            string clientState,
            string userJwt,
            long userJwtExpiresAt)
        {
            if (string.IsNullOrWhiteSpace(clientCallbackUrl))
            {
                return RepositoryResult<(Guid sessionId, int validForSeconds)>.Fail("The client callback url can not be empty.");
            }

            if (string.IsNullOrWhiteSpace(clientState))
            {
                return RepositoryResult<(Guid sessionId, int validForSeconds)>.Fail("The client state can not be empty.");
            }

            if (!await _userInfoService.UserIsAuthenticatedAsync())
            {
                return RepositoryResult<(Guid sessionId, int validForSeconds)>.Fail("There is no user context present in the current request.");
            }

            // This session can be initiated within the next 5 minutes
            var sessionValidityInSeconds = 300;

            var session = new OpenCdeDocumentSelectionSession
            {
                UserId = await _userInfoService.GetCurrentUserIdAsync(),
                ValidUntilUtc = DateTimeOffset.UtcNow.AddSeconds(sessionValidityInSeconds),
                ClientState = clientState,
                ClientCallbackUrl = clientCallbackUrl,
                AuthenticationInformationJson = JsonConvert.SerializeObject(new TokenStorageDto
                {
                    ExpiresAt = userJwtExpiresAt,
                    JsonWebToken = userJwt
                })
            };

            _context.OpenCdeDocumentSelectionSessions.Add(session);
            await _context.SaveChangesAsync();

            return RepositoryResult<(Guid sessionId, int validForSeconds)>.Success((session.Id, sessionValidityInSeconds));
        }

        public async Task<RepositoryResult<DocumentSelectionFinalizationDto>> FinalizeOpenCdeDocumentSelectionAsync(Guid documentSessionId,
            Guid documentId)
        {
            var currentUserId = await _userInfoService
                .GetCurrentUserIdAsync();

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var dbSession = await _context
                .OpenCdeDocumentSelectionSessions
                .FirstOrDefaultAsync(s => s.Id == documentSessionId
                    && s.UserId == currentUserId);
            if (dbSession == null)
            {
                return RepositoryResult<DocumentSelectionFinalizationDto>.Fail("There is no session with the given id for the current user.");
            }

            _context.OpenCdeDocumentSelectionSessions.Remove(dbSession);
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
                ClientState = dbSession.ClientState,
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
    }
}
