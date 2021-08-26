using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Dangl.AspNetCore.FileHandling.Azure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.Data.IO
{
    public class AzureBlobStorageInitializer
    {
        private readonly string _storageConnectionString;

        public AzureBlobStorageInitializer(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString;
        }

        public async Task EnsureAzureBlobStorageHasCorsEnabledAsync()
        {
            var blobServiceClient = new BlobServiceClient(_storageConnectionString);

            var currentProperties = (await blobServiceClient.GetPropertiesAsync()).Value;

            currentProperties.Cors = new List<BlobCorsRule>
            {
                new BlobCorsRule
                {
                    AllowedHeaders = "*",
                    AllowedMethods = "GET,DELETE,PUT,OPTIONS,POST",
                    AllowedOrigins = "*",
                    MaxAgeInSeconds = 3600
                }
            };

            var propertiesUpdateResult = await blobServiceClient.SetPropertiesAsync(currentProperties);
            if (propertiesUpdateResult.Status < 200 || propertiesUpdateResult.Status > 299)
            {
                throw new System.Exception("Could not enable CORS on Azure Blob Storage");
            }
        }

        public async Task EnsureAzureBlobContainersPresentAsync()
        {
            var azureBlobFileManager = new AzureBlobFileManager(_storageConnectionString);

            var containers = new[]
            {
                FileContainerNames.PROJECT_DOCUMENTS
            };

            foreach (var container in containers)
            {
                await azureBlobFileManager.EnsureContainerCreated(container).ConfigureAwait(false);
            }
        }
    }
}
