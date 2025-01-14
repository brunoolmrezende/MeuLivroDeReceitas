using System.Net;
using System.Text.Json;
using CommonTestUtilities.Security;
using FluentAssertions;
using MyRecipeBook.Exceptions;

namespace WebApi.Test.User.Profile
{
    public class GetUserProfileInvalidTokenTest : MyRecipeBookClassFixture
    {
        private readonly string _endpoint = "user";

        public GetUserProfileInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Error_Invalid_Token()
        {
            var response = await DoGet(_endpoint, token: "Invalid token");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Error_Without_Token()
        {
            var response = await DoGet(_endpoint, token: string.Empty);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = ResourceMessagesException.NO_TOKEN;

            errors.Should().ContainSingle().And.Contain(error => error.GetString()!.Equals(expectedMessage));
        }

        [Fact]
        public async Task Error_Token_With_User_NotFound()
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

            var response = await DoGet(_endpoint, token: token);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            var expectedMessage = ResourceMessagesException.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE;

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().ContainSingle().And.Contain(error => error.GetString()!.Equals(expectedMessage));
        }
    }
}
