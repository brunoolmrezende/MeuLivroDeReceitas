using CommonTestUtilities.Requests;
using CommonTestUtilities.Security;
using FluentAssertions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using System.Globalization;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Filter
{
    public class FilterRecipeTest : MyRecipeBookClassFixture
    {
        private readonly string _endpoint = "recipe/filter";
        private readonly Guid _userIdentifier;
        private readonly MyRecipeBook.Domain.Enums.CookingTime _recipeCookingTime;
        private readonly MyRecipeBook.Domain.Enums.Difficulty _recipeDifficulty;
        private readonly string _recipeTitle;
        private readonly IList<MyRecipeBook.Domain.Enums.DishType> _recipeDishType;

        public FilterRecipeTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
            _recipeCookingTime = factory.GetRecipeCookingTime();
            _recipeDishType = factory.GetRecipeDishType();
            _recipeDifficulty = factory.GetRecipeDifficulty();
            _recipeTitle = factory.GetRecipeTitle();
        }

        [Fact]
        public async Task Success()
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var request = new RequestFilterRecipeJson
            {
                CookingTimes = [(MyRecipeBook.Communication.Enums.CookingTime)_recipeCookingTime],
                Difficulties = [(MyRecipeBook.Communication.Enums.Difficulty)_recipeDifficulty],
                DishTypes = _recipeDishType.Select(dishType => (MyRecipeBook.Communication.Enums.DishType)dishType).ToList(),
                RecipeTitle_Ingredient = _recipeTitle,
            };

            var response = await DoPost(endpoint: _endpoint, request: request, token: token);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("recipes").EnumerateArray().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Success_NoContent()
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var request = RequestFilterRecipeJsonBuilder.Build();
            request.RecipeTitle_Ingredient = "recipeDontExist";

            var response = await DoPost(endpoint: _endpoint, request: request, token: token);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_Invalid_CookingTime(string culture)
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var request = RequestFilterRecipeJsonBuilder.Build();
            request.CookingTimes.Add((MyRecipeBook.Communication.Enums.CookingTime)1000);

            var response = await DoPost(endpoint: _endpoint, request: request, token: token, culture: culture);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("COOKING_TIME_NOT_SUPPORTED", new CultureInfo(culture));

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            errors.Should().ContainSingle().And.Contain(e => e.GetString()!.Equals(expectedMessage));
        }
    }
}
