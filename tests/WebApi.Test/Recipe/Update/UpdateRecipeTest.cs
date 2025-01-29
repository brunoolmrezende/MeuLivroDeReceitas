using CommonTestUtilities.Requests;
using CommonTestUtilities.Security;
using FluentAssertions;
using MyRecipeBook.Exceptions;
using System.Globalization;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Update
{
    public class UpdateRecipeTest : MyRecipeBookClassFixture
    {
        private readonly string _endpoint = "recipe";
        private readonly Guid _userIdentifier;
        private readonly string _recipeId;

        public UpdateRecipeTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
            _recipeId = factory.GetRecipeId();
        }

        [Fact]
        public async Task Success()
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var request = RequestRecipeJsonBuilder.Build();

            var response = await DoPut(endpoint: $"{_endpoint}/{_recipeId}", request, token);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Title_Empty(string culture)
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var request = RequestRecipeJsonBuilder.Build();
            request.Title = string.Empty;

            var response = await DoPut(endpoint: $"{_endpoint}/{_recipeId}", request, token, culture);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("RECIPE_TITLE_EMPTY", new CultureInfo(culture));

            errors.Should().ContainSingle().And.Contain(error => error.GetString()!.Equals(expectedMessage));
        }
    }
}
