using Dangl.Data.Shared;
using Dangl.OpenCDE.Core.Extensions;
using Dangl.OpenCDE.Data.Repository;
using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Core.Controllers.CdeApi
{
    [Route("api/opencde/1.0")]
    public class OpenCdeQueryController : CdeAppControllerBase
    {
        private readonly IDocumentsRepository _documentsRepository;

        public OpenCdeQueryController(IDocumentsRepository documentsRepository)
        {
            _documentsRepository = documentsRepository;
        }

        [HttpPost("document-versions")]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(DocumentDiscoverySessionInitialization), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> QueryLatestDocumentVersions([FromBody, Required] DocumentQuery documentQuery)
        {
            if (!(documentQuery?.DocumentIds?.Any() ?? false))
            {
                return BadRequest(new ApiError("At least some ids need to be specified."));
            }

            var documentIds = new List<Guid>();
            foreach (var requestedId in documentQuery.DocumentIds)
            {
                if (Guid.TryParse(requestedId, out var requestedGuid))
                {
                    documentIds.Add(requestedGuid);
                }
            }

            if (documentIds.Count != documentQuery.DocumentIds.Count)
            {
                return BadRequest(new ApiError("Not all given ids were in proper GUID format."));
            }

            var dbList = await _documentsRepository.GetAllDocumentsById(documentIds).ToListAsync();
            var documents = dbList.Select(document => document.ToDocumentVersionForDocument(Url, Request.IsHttps, Request.Host.ToString())).ToList();

            return Ok(documents);
        }
    }
}
