using Dangl.Data.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Client.Controllers
{
    [Route("file-download")]
    public class FileDownloadController: Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FileDownloadController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(FileResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DownloadFileAsync([FromQuery] string downloadUrl)
        {
            if (string.IsNullOrWhiteSpace(downloadUrl))
            {
                return BadRequest(new ApiError("Missing download url"));
            }

            var bearerToken = string.Empty;
            if (Request.Headers.TryGetValue("Authorization", out var authHeaders)
                && authHeaders.Count == 1)
            {
                var authHeader = authHeaders.First();
                if (authHeader.StartsWith("Bearer "))
                {
                    bearerToken = authHeader.Substring("Bearer ".Length);
                }
            }

            using var httpClient = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, downloadUrl);
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
            }

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new ApiError($"Status code: {response.StatusCode}{Environment.NewLine}{await response.Content.ReadAsStringAsync()}"));
            }

            var responseContent = await response.Content.ReadAsStreamAsync();
            var fileName = response.Content.Headers.ContentDisposition?.FileName ?? "file";
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

            return File(responseContent, contentType, fileName);
        }
    }
}
