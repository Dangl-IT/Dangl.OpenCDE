﻿using Dangl.OpenCDE.Core.Configuration;
using Dangl.OpenCDE.Shared.Models.Foundations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;

namespace Dangl.OpenCDE.Core.Controllers.Foundations
{
    [Route("foundation/versions")]
    [AllowAnonymous]
    [ApiController]
    public class VersionsController : ControllerBase
    {
        private readonly OpenCdeSettings _settings;

        public VersionsController(OpenCdeSettings settings)
        {
            _settings = settings;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(VersionsGet), (int)HttpStatusCode.OK)]
        public IActionResult GetApiVersions()
        {
            var versions = new VersionsGet
            {
                Versions = new List<VersionGet>
                {
                    new VersionGet
                    {
                        ApiId = "foundation",
                        VersionId = "1.0",
                        DetailedVersion = "https://github.com/BuildingSMART/foundation-API/tree/release_1_0",
                        ApiBaseUrl = GetAbsolutePath("foundation/1.0")
                    },
                    new VersionGet
                    {
                        ApiId = "documents",
                        VersionId = "1.0",
                        ApiBaseUrl = GetAbsolutePath("api/opencde/1.0")
                    }
                }
            };

            return Ok(versions);
        }

        private string GetAbsolutePath(string relativePath)
        {
            return $"{_settings.AppBaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        }
    }
}
