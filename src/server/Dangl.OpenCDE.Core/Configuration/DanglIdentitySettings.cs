using Dangl.OpenCDE.Shared.Configuration;
using System;
using System.Net.Http;

namespace Dangl.OpenCDE.Core.Configuration
{
    public class DanglIdentitySettings
    {
        public string ClientId { get; set; }

        /// <summary>
        /// The client secret is optional, it's currently not used in the app since
        /// the authentication is just using the implicit flow
        /// </summary>
        public string ClientSecret { get; set; }

        public string BaseUri { get; set; }

        public string RequiredScope { get; set; }

        public Func<HttpMessageHandler> CustomBackchannelHttpMessageHandlerFactory { get; set; }

        public bool UseDefaultInMemoryUserUpdaterCache { get; set; } = true;

        // This should only ever be used for testing purposes and never be enabled in production
        public bool AllowInsecureJwtIssuers { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                throw new InvalidConfigurationException($"{nameof(ClientId)} missing.");
            }

            if (string.IsNullOrWhiteSpace(BaseUri))
            {
                throw new InvalidConfigurationException($"{nameof(BaseUri)} missing.");
            }

            if (!Uri.TryCreate(BaseUri, UriKind.Absolute, out var _))
            {
                throw new InvalidConfigurationException($"{nameof(BaseUri)} must be an absolute url.");
            }

            if (string.IsNullOrWhiteSpace(RequiredScope))
            {
                throw new InvalidConfigurationException($"{nameof(RequiredScope)} missing.");
            }
        }
    }
}
