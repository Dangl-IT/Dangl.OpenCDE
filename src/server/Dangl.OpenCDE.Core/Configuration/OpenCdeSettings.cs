using Dangl.OpenCDE.Shared.Configuration;
using System;

namespace Dangl.OpenCDE.Core.Configuration
{
    public class OpenCdeSettings
    {
        public DanglIdentitySettings DanglIdentitySettings { get; set; }

        public StorageSettings StorageSettings { get; set; }

        public string ApplicationInsightsInstrumentationKey { get; set; }

        public string AppBaseUrl { get; set; }

        public string DanglIconsBaseUrl { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(AppBaseUrl))
            {
                throw new InvalidConfigurationException($"{nameof(AppBaseUrl)} missing.");
            }

            if (!AppBaseUrl.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
                && !AppBaseUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidConfigurationException($"{nameof(AppBaseUrl)} must be an absolute uri and use http or https");
            }

            if (string.IsNullOrWhiteSpace(DanglIconsBaseUrl))
            {
                throw new InvalidConfigurationException($"{nameof(DanglIconsBaseUrl)} missing.");
            }

            if (!DanglIconsBaseUrl.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
                && !DanglIconsBaseUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidConfigurationException($"{nameof(DanglIconsBaseUrl)} must be an absolute uri and use http or https");
            }

            if (DanglIdentitySettings == null)
            {
                throw new InvalidConfigurationException($"{nameof(DanglIdentitySettings)} missing.");
            }

            DanglIdentitySettings?.Validate();
            if (StorageSettings == null)
            {
                throw new InvalidConfigurationException($"{nameof(StorageSettings)} missing.");
            }

            StorageSettings?.Validate();
        }
    }
}
