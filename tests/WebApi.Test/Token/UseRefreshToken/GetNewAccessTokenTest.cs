using FluentAssertions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Token.UseRefreshToken
{
    public class GetNewAccessTokenTest : MyRecipeBookClassFixture
    {
        private const string _endpoint = "token";

        private readonly string _refreshToken;

        public GetNewAccessTokenTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _refreshToken = factory.GetRefreshToken();
        }

        [Fact]
        public async Task Sucess()
        {
            var request = new RequestNewTokenJson
            {
                RefreshToken = _refreshToken,
            };

            var response = await DoPost($"{_endpoint}/refresh-token", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("accessToken").GetString().Should().NotBeNullOrWhiteSpace();
            responseData.RootElement.GetProperty("refreshToken").GetString().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Invalid_Login(string culture)
        {
            var request = new RequestNewTokenJson
            {
                RefreshToken = "InvalidToken"
            };

            var respone = await DoPost(endpoint: $"{_endpoint}/refresh-token", request: request, culture: culture);

            respone.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            await using var responseBody = await respone.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("EXPIRED_SESSION", new CultureInfo(culture));

            errors.Should().ContainSingle().And.Contain(error => error.GetString()!.Equals(expectedMessage));
        }
    }
}
