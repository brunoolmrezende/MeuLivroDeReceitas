using CommonTestUtilities.IdEncrypter;
using CommonTestUtilities.Security;
using FluentAssertions;
using MyRecipeBook.Exceptions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.GetById
{
    public class GetRecipeByIdTest : MyRecipeBookClassFixture
    {
        private readonly string _endpoint = "recipe";
        private readonly Guid _userIdentifier;
        private readonly string _recipeId;
        private readonly string _recipeTitle;

        public GetRecipeByIdTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
            _recipeId = factory.GetRecipeId();
            _recipeTitle = factory.GetRecipeTitle();
        }

        [Fact]
        public async Task Success()
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoGet(endpoint: $"{_endpoint}/{_recipeId}", token: token);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using var responsebody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responsebody);

            responseData.RootElement.GetProperty("id").GetString().Should().Be(_recipeId);
            responseData.RootElement.GetProperty("title").GetString().Should().Be(_recipeTitle);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Recipe_Not_Found(string culture)
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var id = IdEncrypterBuilder.Build().Encode(1000);

            var response = await DoGet(endpoint: $"{_endpoint}/{id}", token: token, culture: culture);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            using var responsebody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responsebody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("RECIPE_NOT_FOUND", new CultureInfo(culture));

            errors.Should().ContainSingle().And.Contain(e => e.GetString()!.Equals(expectedMessage));
        }
    }
}
