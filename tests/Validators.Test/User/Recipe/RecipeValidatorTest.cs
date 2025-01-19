using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Exceptions;

namespace Validators.Test.User.Recipe
{
    public class RecipeValidatorTest
    {
        [Fact]
        public void Success()
        {
            var request = RequestRecipeJsonBuilder.Build();

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Error_Invalid_CookingTime()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.CookingTime = (MyRecipeBook.Communication.Enums.CookingTime?)1000;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result?.IsValid.Should().BeFalse();
            result?.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.COOKING_TIME_NOT_SUPPORTED));
        }

        [Fact]
        public void Error_Invalid_Difficulty()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Difficulty = (MyRecipeBook.Communication.Enums.Difficulty?)1000;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result?.IsValid.Should().BeFalse();
            result?.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.DIFFICULTY_LEVEL_NOT_SUPPORTED));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void Error_Invalid_Title(string title)
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Title = title;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result?.IsValid.Should().BeFalse();
            result?.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.RECIPE_TITLE_EMPTY));
        }

        [Fact]
        public void Success_Empty_CookingTime()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.CookingTime = null;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result?.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Success_Empty_Difficulty()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Difficulty = null;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result?.IsValid.Should().BeTrue();
        }
    }
}
