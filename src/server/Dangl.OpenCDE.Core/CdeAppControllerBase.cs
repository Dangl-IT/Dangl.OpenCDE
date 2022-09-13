using AutoMapper;
using Dangl.Data.Shared;
using Dangl.OpenCDE.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Dangl.OpenCDE.Core
{
    [Authorize]
    [ApiController]
    public class CdeAppControllerBase : ControllerBase
    {
        protected IActionResult FromRepositoryResult<TValue>(RepositoryResult<TValue> repoResult, bool toNotFoundWhenValueIsNull = false)
        {
            if (toNotFoundWhenValueIsNull && repoResult.Value == null)
            {
                return NotFound();
            }

            if (!repoResult.IsSuccess)
            {
                return BadRequest(new ApiError(repoResult.ErrorMessage));
            }

            return Ok(repoResult.Value);
        }

        protected IActionResult FromRepositoryResult<TValue, TModel>(RepositoryResult<TValue> repoResult, IMapper mapper)
        {
            if (!repoResult.IsSuccess)
            {
                return BadRequest(new ApiError(repoResult.ErrorMessage));
            }

            return Ok(mapper.Map<TModel>(repoResult.Value));
        }

        protected IActionResult FromRepositoryResult<TValue, TError, TModel>(RepositoryResult<TValue, TError> repoResult, IMapper mapper)
        {
            if (!repoResult.IsSuccess)
            {
                return BadRequest(new ApiError<TError>(repoResult.ErrorMessage, repoResult.Error));
            }

            return Ok(mapper.Map<TModel>(repoResult.Value));
        }

        protected IActionResult FromRepositoryResult(RepositoryResult repoResult)
        {
            if (!repoResult.IsSuccess)
            {
                return BadRequest(new ApiError(repoResult.ErrorMessage));
            }

            return NoContent();
        }

        protected IActionResult CreatedFromRepositoryResult<TValue, TModel>(RepositoryResult<TValue> repoResult, string controllerName, Func<object> routeValuesProvider, IMapper mapper)
        {
            if (!repoResult.IsSuccess)
            {
                return BadRequest(new ApiError(repoResult.ErrorMessage));
            }

            var result = mapper.Map<TModel>(repoResult.Value);
            return CreatedAtAction(controllerName.WithoutAsyncSuffix(),
                routeValuesProvider(),
                result);
        }
    }
}
