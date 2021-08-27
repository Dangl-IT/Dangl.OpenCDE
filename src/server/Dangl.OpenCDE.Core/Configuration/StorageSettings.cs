using Dangl.OpenCDE.Shared.Configuration;

namespace Dangl.OpenCDE.Core.Configuration
{
    public class StorageSettings
    {
        public bool UseCustomFileManager { get; set; }

        public string AzureBlobFileManagerConnectionString { get; set; }

        public string AzureBlobStorageLogConnectionString { get; set; }

        public void Validate()
        {
            if (!UseCustomFileManager && string.IsNullOrWhiteSpace(AzureBlobFileManagerConnectionString))
            {
                throw new InvalidConfigurationException($"The {nameof(AzureBlobFileManagerConnectionString)} must be set when {nameof(UseCustomFileManager)} is set to false.");
            }
        }
    }
}
