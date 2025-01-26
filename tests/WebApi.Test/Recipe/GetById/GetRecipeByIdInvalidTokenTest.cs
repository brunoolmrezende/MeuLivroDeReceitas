using CommonTestUtilities.Security;
using FluentAssertions;
using MyRecipeBook.Exceptions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.GetById
{
    public class GetRecipeByIdInvalidTokenTest : MyRecipeBookClassFixture
    {
        private readonly string _endpoint = "recipe";
        private readonly string _recipeId;

        public GetRecipeByIdInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _recipeId = factory.GetRecipeId();
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Without_Token(string culture)
        {
            var token = string.Empty;

            var response = await DoGet(endpoint: $"{_endpoint}/{_recipeId}", token: token, culture: culture);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            using var responsebody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responsebody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("NO_TOKEN", new CultureInfo(culture));

            errors.Should().ContainSingle().And.Contain(e => e.GetString()!.Equals(expectedMessage));
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Invalid_Token(string culture)
        {
            var response = await DoGet(endpoint: $"{_endpoint}/{_recipeId}", token: "InvalidToken", culture: culture);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            using var responsebody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responsebody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("USER_WITHOUT_PERMISSION_ACCESS_RESOURCE", new CultureInfo(culture));

            errors.Should().ContainSingle().And.Contain(e => e.GetString()!.Equals(expectedMessage));
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Token_With_User_Not_Found(string culture)
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

            var response = await DoGet(endpoint: $"{_endpoint}/{_recipeId}", token: token, culture: culture);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            using var responsebody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responsebody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("USER_WITHOUT_PERMISSION_ACCESS_RESOURCE", new CultureInfo(culture));

            errors.Should().ContainSingle().And.Contain(e => e.GetString()!.Equals(expectedMessage));
        }
        
    }
}
