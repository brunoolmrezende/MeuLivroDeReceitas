using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Generate;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exceptions;

namespace Validators.Test.Recipe.Generate
{
    public class GenerateRecipeValidatorTest
    {
        [Fact]
        public void Success()
        {
            var request = RequestGenerateRecipeJsonBuilder.Build();

            var validator = new GenerateRecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Error_More_Maximum_Ingredient()
        {
            var request = RequestGenerateRecipeJsonBuilder
                .Build(MyRecipeBookRuleConstants.MAXIMUM_INGREDIENTS_GENERATE_RECIPE + 1);

            var validator = new GenerateRecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.INVALID_NUMBER_INGREDIENTS));
        }

        [Fact]
        public void Error_Duplicated_Ingredient()
        {
            var request = RequestGenerateRecipeJsonBuilder
                .Build(MyRecipeBookRuleConstants.MAXIMUM_INGREDIENTS_GENERATE_RECIPE - 1);
            request.Ingredients.Add(request.Ingredients[0]);

            var validator = new GenerateRecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.DUPLICATED_INGREDIENTS_IN_LIST));
        }

        [Fact]
        public void Error_Ingredient_Not_Following_Pattern()
        {
            var request = RequestGenerateRecipeJsonBuilder
                .Build(MyRecipeBookRuleConstants.MAXIMUM_INGREDIENTS_GENERATE_RECIPE - 2);

            request.Ingredients.Add("This is an invalid ingredient because it is too long");

            var validator = new GenerateRecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.INGREDIENT_NOT_FOLLOWING_PATTERN));
        }

        [Theory]
        [InlineData("            ")]
        [InlineData("")]
        [InlineData(null)]
        public void Error_Empty_Ingredient(string ingredient)
        {
            var request = RequestGenerateRecipeJsonBuilder
                .Build(MyRecipeBookRuleConstants.MAXIMUM_INGREDIENTS_GENERATE_RECIPE - 2);

            request.Ingredients.Add(ingredient);

            var validator = new GenerateRecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.INGREDIENT_EMPTY));
        }
    }
}
