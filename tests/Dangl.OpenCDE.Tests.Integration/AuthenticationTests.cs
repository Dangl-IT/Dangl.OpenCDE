using Dangl.Identity.Client.Models;
using Dangl.OpenCDE.TestUtilities.BaseTests;
using Dangl.OpenCDE.TestUtilities.TestData;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Dangl.OpenCDE.Tests.Integration
{
    public class AuthenticationTests : PostControllerTestBase<TokenLoginPost, TokenResponseGet>
    {
        public AuthenticationTests(TestUtilities.SqlServerDockerCollectionFixture fixture)
            : base(fixture)
        {
            _payload = new TokenLoginPost
            {
                Identifier = Users.NotKnownBeforeUser.Username,
                Password = Users.NotKnownBeforeUser.Password
            };
        }

        protected override Task<HttpResponseMessage> MakeRequest()
        {
            _client = _testHelper.GetAnonymousClient();
            return base.MakeRequest();
        }

        protected override string GetUrl()
        {
            return "/identity/token-login";
        }

        [Fact]
        public async Task CanLoginWithUsername()
        {
            _payload.Identifier = Users.User.Username;
            await MakeRequest();
            Assert.True(_response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task CanLoginWithEmail()
        {
            _payload.Identifier = Users.User.Email;
            await MakeRequest();
            Assert.True(_response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task CreatesUserInDatabaseAfterLogin()
        {
            await using (var context = _testHelper.GetNewCdeDbContext())
            {
                var usersPresent = await context.Users.AnyAsync(u => u.Id == Users.NotKnownBeforeUser.UserId);
                Assert.False(usersPresent);
            }

            await MakeRequest();

            await using (var context = _testHelper.GetNewCdeDbContext())
            {
                var usersPresent = await context.Users.AnyAsync(u => u.Id == Users.NotKnownBeforeUser.UserId);
                Assert.True(usersPresent);
            }
        }

        [Fact]
        public async Task CanNotLoginWithIncorrectPassword()
        {
            _payload.Password = "Incorrect";
            await MakeRequest();
            Assert.False(_response.IsSuccessStatusCode);
        }
    }
}
