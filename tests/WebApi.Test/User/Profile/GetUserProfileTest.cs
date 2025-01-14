using System.Net;
using System.Text.Json;
using CommonTestUtilities.Security;
using FluentAssertions;

namespace WebApi.Test.User.Profile
{
    public class GetUserProfileTest : MyRecipeBookClassFixture
    {
        private readonly string _endpoint = "user";

        private readonly string _email;
        private readonly string _name;
        private readonly Guid _userIdentifier;

        public GetUserProfileTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _email = factory.GetEmail();
            _name = factory.GetName();
            _userIdentifier = factory.GetUserIdentifier();
        }

        [Fact]
        public async Task Success()
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoGet(_endpoint, token: token);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("name").GetString().Should().NotBeNull().And.Be(_name);
            responseData.RootElement.GetProperty("email").GetString().Should().NotBeNull().And.Be(_email);
        }
    }
}
