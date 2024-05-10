using Dangl.Identity.TestHost.SetupData;
using Duende.IdentityServer.Models;
using System.Collections.Generic;
using System.Linq;

namespace Dangl.OpenCDE.TestUtilities.TestData
{
    public static class Clients
    {
        public static List<ClientSetupDto> Values => new List<ClientSetupDto>
        {
            OpenCdeAppClient
        };

        public static ClientSetupDto OpenCdeAppClient => new ClientSetupDto
        {
            Id = 1,
            ClientId = "dc301212-4560-4efc-a970-bb2dcedc7361",
            ClientSecret = "WholeWalnutCeiling",
            Uri = "https://www.dangl-it.com",
            RedirectUri = "https://www.example.com",
            Name = "CdeIntegrationTestClient",
            AllowedScopes = new List<string> { IntegrationTestConstants.REQUIRED_SCOPE },
            AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials.ToList(),
            Description = "Just a test client for integration testing",
            AllowsDelegatedAccountAccess = true
        };
    }
}
