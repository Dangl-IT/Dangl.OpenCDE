using Dangl.Data.Shared;
using Dangl.Identity.Client.Mvc.Services;
using Dangl.OpenCDE.Core.Utilities;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Data.Models;
using Dangl.OpenCDE.Data.Repository;
using Dangl.OpenCDE.Shared.Models.CdeApi;
using Dangl.OpenCDE.Shared.Models.Controllers.OpenCdeIntegration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Dangl.OpenCDE.Core.Controllers
{
    [Route("api/open-cde-integration")]
    public class OpenCdeIntegrationController : CdeAppControllerBase
    {
        private readonly SignInManager<CdeUser> _signInManager;
        private readonly UserManager<CdeUser> _userManager;
        private readonly IOpenCdeDocumentSelectionRepository _openCdeDocumentSelectionService;
        private readonly IDocumentsRepository _documentsRepository;
        private readonly IUserInfoService _userInfoService;

        public OpenCdeIntegrationController(SignInManager<CdeUser> signInManager,
            UserManager<CdeUser> userManager,
            IOpenCdeDocumentSelectionRepository openCdeDocumentSelectionService,
            IDocumentsRepository documentsRepository,
            IUserInfoService userInfoService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _openCdeDocumentSelectionService = openCdeDocumentSelectionService;
            _documentsRepository = documentsRepository;
            _userInfoService = userInfoService;
        }

        [AllowAnonymous]
        [HttpGet("sessions/{documentSessionId}/simple-auth")]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(SimpleAuthToken), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSessionSimpleAuthDataAsync(Guid documentSessionId)
        {
            var userId = await _openCdeDocumentSelectionService.GetUserSessionDataForDocumentSessionAsync(documentSessionId);
            if (!userId.IsSuccess)
            {
                return BadRequest(new ApiError(userId.ErrorMessage));
            }

            return Ok(new SimpleAuthToken
            {
                Jwt = userId.Value.tokenStorage.JsonWebToken,
                ExpiresAt = userId.Value.tokenStorage.ExpiresAt
            });
        }

        [HttpPost("sessions/{documentSessionId}/document-selection")]
        [ProducesResponseType(typeof(DocumentSelectionGet), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetDocumentSelection(Guid documentSessionId, [FromBody] DocumentSelectionPost documentSelection)
        {
            var repoResult = await _openCdeDocumentSelectionService
                .FinalizeOpenCdeDocumentSelectionAsync(documentSessionId, documentSelection.DocumentId);
            if (!repoResult.IsSuccess)
            {
                return BadRequest(new ApiError(repoResult.ErrorMessage));
            }

            var selectedDocumentsUrl = Url.Action(nameof(GetDocumentSelectionDataAsync).WithoutAsyncSuffix(),
                nameof(OpenCdeIntegrationController).WithoutControllerSuffix(), new
                {
                    documentSelectionId = repoResult.Value.SelectionId
                }, Request.IsHttps ? "https" : "http", Request.Host.ToString());

            var url = new Uri(repoResult.Value.ClientCallbackUrl);
            var query = HttpUtility.ParseQueryString(url.Query);
            query.Add("selected_documents_url", selectedDocumentsUrl);
            query.Add("selection_context", repoResult.Value.SelectionId.ToString());
            var uriBuilder = new UriBuilder(url);
            uriBuilder.Query = query.ToString();
            var callbackUrl = uriBuilder.ToString();

            return Ok(new DocumentSelectionGet
            {
                CallbackUrl = callbackUrl
            });
        }

        [HttpGet("documents/{documentId}/metadata")]
        [ProducesResponseType(typeof(DocumentMetadata), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetDocumentMetadataAsync(Guid documentId)
        {
            var document = await _documentsRepository
                .GetDocumentByIdAsync(documentId);

            if (!document.IsSuccess)
            {
                return BadRequest(new ApiError(document.ErrorMessage));
            }

            var metadata = new DocumentMetadata
            {
                Entries = new System.Collections.Generic.List<DocumentMetadataEntry>
                {
                    new DocumentMetadataEntry
                    {
                        Name = nameof(document.Value.Name),
                        Value = document.Value.Name,
                        DataType = DocumentMetadataDataType.String
                    },
                    new DocumentMetadataEntry
                    {
                        Name = nameof(document.Value.FileName),
                        Value = document.Value.FileName,
                        DataType = DocumentMetadataDataType.String
                    },
                    new DocumentMetadataEntry
                    {
                        Name = nameof(document.Value.CreatedAtUtc),
                        Value = document.Value.CreatedAtUtc.ToString("O"),
                        DataType = DocumentMetadataDataType.DateTime
                    },
                    new DocumentMetadataEntry
                    {
                        Name = nameof(document.Value.FileSizeInBytes),
                        Value = document.Value.FileSizeInBytes.ToString(),
                        DataType = DocumentMetadataDataType.Integer
                    }
                },
                Links = new DocumentMetadataLinks
                {
                    Self = new LinkData
                    {
                        Href = GetAbsoluteBaseUrl(nameof(OpenCdeIntegrationController), nameof(GetDocumentMetadataAsync), new
                        {
                            documentId
                        })
                    },
                    DocumentReference = new LinkData
                    {
                        Href = GetAbsoluteBaseUrl(nameof(OpenCdeIntegrationController), nameof(GetDocumentReferenceAsync), new
                        {
                            documentId
                        })
                    }
                }
            };

            return Ok(metadata);
        }

        [HttpGet("documents/{documentId}/versions")]
        [ProducesResponseType(typeof(DocumentVersions), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetDocumentVersionsAsync(Guid documentId)
        {
            var document = await _documentsRepository
                .GetDocumentByIdAsync(documentId);

            if (!document.IsSuccess)
            {
                return BadRequest(new ApiError(document.ErrorMessage));
            }

            var documentReference = GetDocumentReferenceForDocument(document.Value);

            var documentVersions = new DocumentVersions
            {
                Links = new DocumentVersionLinks
                {
                    Self = new LinkData
                    {
                        Href = GetAbsoluteBaseUrl(nameof(OpenCdeIntegrationController), nameof(GetDocumentVersionsAsync), new
                        {
                            documentId
                        })
                    }
                },
                DocumentReferences = new DocumentVersionsEmbeddedReferences
                {
                    DocumentReferences = new System.Collections.Generic.List<DocumentReference>
                    {
                        documentReference
                    }
                }
            };

            return Ok(documentVersions);
        }

        [HttpGet("documents/{documentId}/reference")]
        [ProducesResponseType(typeof(SelectedDocuments), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetDocumentReferenceAsync(Guid documentId)
        {
            var document = await _documentsRepository
                .GetDocumentByIdAsync(documentId);

            if (!document.IsSuccess)
            {
                return BadRequest(new ApiError(document.ErrorMessage));
            }

            var documentReference = GetDocumentReferenceForDocument(document.Value);
            var selection = new SelectedDocuments
            {
                DocumentReferences = new System.Collections.Generic.List<DocumentReference> { documentReference }
            };

            return Ok(selection);
        }

        [HttpGet("document-selections/{documentSelectionId}")]
        [ProducesResponseType(typeof(DocumentReference), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetDocumentSelectionDataAsync(Guid documentSelectionId)
        {
            var document = await _openCdeDocumentSelectionService.GetDocumentForSelectionAsync(documentSelectionId);
            if (!document.IsSuccess)
            {
                return BadRequest(new ApiError(document.ErrorMessage));
            }

            var documentReference = GetDocumentReferenceForDocument(document.Value);
            var selection = new SelectedDocuments
            {
                DocumentReferences = new System.Collections.Generic.List<DocumentReference> { documentReference }
            };

            return Ok(selection);
        }

        private DocumentReference GetDocumentReferenceForDocument(DocumentDto document)
        {
            return new DocumentReference
            {
                Title = document.Name,
                Version = "1",
                VersionDate = document.CreatedAtUtc,
                FileDescription = new FileDescription
                {
                    Name = document.FileName,
                    FileSizeInBytes = document.FileSizeInBytes ?? 0
                },
                Links = new DocumentReferenceLinks
                {
                    Content = new LinkData
                    {
                        Href = GetAbsoluteBaseUrl(nameof(DocumentsController), nameof(DocumentsController.DownloadDocumentAsync), new
                        {
                            projectId = document.ProjectId,
                            documentId = document.Id
                        })
                    },
                    Metadata = new LinkData
                    {
                        Href = GetAbsoluteBaseUrl(nameof(OpenCdeIntegrationController), nameof(GetDocumentMetadataAsync), new
                        {
                            documentId = document.Id
                        })
                    },
                    Self = new LinkData
                    {
                        Href = GetAbsoluteBaseUrl(nameof(OpenCdeIntegrationController), nameof(GetDocumentReferenceAsync), new
                        {
                            documentId = document.Id
                        })
                    },
                    Versions = new LinkData
                    {
                        Href = GetAbsoluteBaseUrl(nameof(OpenCdeIntegrationController), nameof(GetDocumentVersionsAsync), new
                        {
                            documentId = document.Id
                        })
                    }
                }
            };
        }

        private string GetAbsoluteBaseUrl(string controllerName, string actionName, object routeParameters)
        {
            controllerName = controllerName.WithoutControllerSuffix();
            actionName = actionName.WithoutAsyncSuffix();
            var protocol = Request.IsHttps ? "https" : "http";

            var url = Url.Action(actionName, controllerName, routeParameters, protocol, Request.Host.ToString(), null);
            return url;
        }
    }
}
