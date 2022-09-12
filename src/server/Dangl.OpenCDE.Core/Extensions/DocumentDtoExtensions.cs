using Azure.Core;
using Dangl.OpenCDE.Core.Controllers;
using Dangl.OpenCDE.Core.Controllers.CdeApi;
using Dangl.OpenCDE.Core.Utilities;
using Dangl.OpenCDE.Data.Dto.Documents;
using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Core.Extensions
{
    public static class DocumentDtoExtensions
    {
        public static DocumentVersion ToDocumentVersionForDocument(this DocumentDto document,
            IUrlHelper urlHelper,
            bool isHttps,
            string host)
        {
            return new DocumentVersion
            {
                Title = document.Name,
                VersionNumber = "1",
                CreationDate = document.CreatedAtUtc,
                FileDescription = new FileDescription
                {
                    Name = document.FileName,
                    SizeInBytes = document.FileSizeInBytes ?? 0
                },
                Links = new DocumentVersionLinks
                {
                    DocumentVersionDownload = new LinkData
                    {
                        Url = GetAbsoluteBaseUrl(nameof(DocumentsController),
                        nameof(DocumentsController.DownloadDocumentAsync), new
                        {
                            projectId = document.ProjectId,
                            documentId = document.Id
                        },
                        urlHelper,
                        isHttps,
                        host)
                    },
                    DocumentVersionMetadata = new LinkData
                    {
                        Url = GetAbsoluteBaseUrl(nameof(OpenCdeDownloadIntegrationController),
                        nameof(OpenCdeDownloadIntegrationController.GetDocumentMetadataAsync), new
                        {
                            documentId = document.Id
                        },
                        urlHelper,
                        isHttps,
                        host)
                    },
                    DocumentVersion = new LinkData
                    {
                        Url = GetAbsoluteBaseUrl(nameof(OpenCdeDownloadIntegrationController),
                        nameof(OpenCdeDownloadIntegrationController.GetDocumentReferenceAsync), new
                        {
                            documentId = document.Id
                        },
                        urlHelper,
                        isHttps,
                        host)
                    },
                    DocumentVersions = new LinkData
                    {
                        Url = GetAbsoluteBaseUrl(nameof(OpenCdeDownloadIntegrationController),
                        nameof(OpenCdeDownloadIntegrationController.GetDocumentVersionsAsync), new
                        {
                            documentId = document.Id
                        },
                        urlHelper,
                        isHttps,
                        host)
                    }
                }
            };
        }
        
        private static string GetAbsoluteBaseUrl(string controllerName,
            string actionName,
            object routeParameters,
            IUrlHelper urlHelper,
            bool isHttps,
            string host)
        {
            controllerName = controllerName.WithoutControllerSuffix();
            actionName = actionName.WithoutAsyncSuffix();
            var protocol = isHttps ? "https" : "http";

            var url = urlHelper.Action(actionName, controllerName, routeParameters, protocol, host, null);
            return url;
        }
    }
}
