using Dangl.OpenCDE.Client.Hubs;
using Dangl.OpenCDE.Client.Models;
using Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Dangl.OpenCDE.Shared.OpenCdeSwaggerGenerated.Models.UploadFilePartInstruction;

namespace Dangl.OpenCDE.Client.Services
{
    public class OpenCdeUploadService
    {
        private readonly ClientNotificationsService _clientNotificationsService;
        private readonly IHubContext<CdeClientHub> _hubContext;
        private readonly HttpClient _httpClient;

        public OpenCdeUploadService(ClientNotificationsService clientNotificationsService,
            IHubContext<CdeClientHub> hubContext,
            HttpClient httpClient)
        {
            _clientNotificationsService = clientNotificationsService;
            _hubContext = hubContext;
            _httpClient = httpClient;
        }

        public async Task PerformUploadAsync(OpenCdeUploadTask uploadTask)
        {
            foreach (var file in uploadTask.CdeUploadInstructions._DocumentsToUpload)
            {
                var originalFile = uploadTask.ClientFileData
                        .Files
                        .FirstOrDefault(f => f.SessionFileId == file.SessionFileId);

                if (originalFile == null)
                {
                    await _clientNotificationsService.SendErrorToClientAsync($"The file with id {file.SessionFileId} could not be uploaded, it was present in the server instructions but not locally selected to upload.");
                    continue;
                }

                    await UploadSingleFile(file,
                        originalFile.FilePath,
                        uploadTask.ClientFileData.AccessToken);
            }
        }

        private async Task UploadSingleFile(DocumentToUpload cdeInstructions,
            string localPath,
            string accessToken)
        {
            var fileName = Path.GetFileName(localPath);
            await _clientNotificationsService.SendInformationToClientAsync($"Uploading file {fileName} ...");
            await using var sourceFileStream = System.IO.File.OpenRead(localPath);

            foreach (var uploadPart in cdeInstructions.UploadFileParts)
            {
                var uploadRequest = new HttpRequestMessage();
                uploadRequest.Method = uploadPart.HttpMethod switch
                {
                    HttpMethodEnum.PUTEnum => HttpMethod.Put,
                    HttpMethodEnum.POSTEnum => HttpMethod.Post,
                    _ => throw new NotImplementedException()
                };

                uploadRequest.RequestUri = new Uri(uploadPart.Url);

                if (uploadPart.IncludeAuthorization)
                {
                    uploadRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                }

                var customContentType = string.Empty;
                if (uploadPart.AdditionalHeaders?.Values?.Any() ?? false)
                {
                    foreach (var header in uploadPart.AdditionalHeaders.Values)
                    {
                        if (header.Name.Equals("content-length", StringComparison.InvariantCultureIgnoreCase))
                        {
                            // Some systems add that, but we're skipping it
                            continue;
                        }

                        if (header.Name.Equals("content-type", StringComparison.InvariantCultureIgnoreCase))
                        {
                            // This is handled differently, directly in the request
                            customContentType = header.Value;
                            continue;
                        }

                        uploadRequest.Headers.Add(header.Name, header.Value);
                    }
                }

                using var uploadStream = new MemoryStream();

                if (uploadPart.MultipartFormData?.Prefix != null)
                {
                    var prefixStream = new MemoryStream(uploadPart.MultipartFormData.Prefix);
                    await prefixStream.CopyToAsync(uploadStream);
                }

                await CopyPartialStreamAsync(sourceFileStream,
                    uploadStream,
                    uploadPart.ContentRangeStart,
                    uploadPart.ContentRangeEnd);

                if (uploadPart.MultipartFormData?.Suffix != null)
                {
                    var prefixStream = new MemoryStream(uploadPart.MultipartFormData.Suffix);
                    await prefixStream.CopyToAsync(uploadStream);
                }

                uploadStream.Position = 0;
                uploadRequest.Content = new StreamContent(uploadStream);

                if (!string.IsNullOrWhiteSpace(customContentType))
                {
                    uploadRequest.Content.Headers.Remove("Content-Type");
                    uploadRequest.Content.Headers.TryAddWithoutValidation("Content-Type", customContentType);
                }

                var response = await _httpClient.SendAsync(uploadRequest);
            }


            var completionRequest = new HttpRequestMessage(HttpMethod.Post, cdeInstructions.UploadCompletion.Url);
            completionRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var completionResponse = await _httpClient.SendAsync(completionRequest);
            if (completionResponse.IsSuccessStatusCode)
            {
                await _clientNotificationsService.SendInformationToClientAsync($"Finished uploading file {fileName}.");
                var completionJson = await completionResponse.Content.ReadAsStringAsync();
                var documentVersion = JsonConvert.DeserializeObject<DocumentVersion>(completionJson);
                await _hubContext.Clients
                    .All
                    .SendAsync("DocumentVersionUploaded", documentVersion);
            }
            else
            {
                await _clientNotificationsService.SendErrorToClientAsync("Received an error when announcing the upload completion on the cde" +
                    ", the response status code was: "
                            + completionResponse.StatusCode
                            + " (" + (int)completionResponse.StatusCode + ")");
            }
        }

        public static async Task CopyPartialStreamAsync(Stream input, Stream output, long startFrom, long copyUntil)
        {
            // Taken basically from https://stackoverflow.com/a/13022108/4190785
            input.Seek(startFrom, SeekOrigin.Begin);

            var buffer = new byte[32768];
            int read;
            while (copyUntil > 0 &&
                   (read = await input.ReadAsync(buffer, 0, Math.Min(buffer.Length, Convert.ToInt32(copyUntil)))) > 0)
            {
                await output.WriteAsync(buffer, 0, read);
                copyUntil -= read;
            }
        }
    }
}
