using Bogus;
using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;

namespace Validators.Test.Recipe
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
        public void Error_CookingTime_Invalid()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.CookingTime = (MyRecipeBook.Communication.Enums.CookingTime?)1000;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result?.IsValid.Should().BeFalse();
            result?.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.COOKING_TIME_NOT_SUPPORTED));
        }

        [Fact]
        public void Error_Difficulty_Invalid()
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
        public void Error_Title_Invalid(string title)
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Title = title;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result?.IsValid.Should().BeFalse();
            result?.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.RECIPE_TITLE_EMPTY));
        }

        [Fact]
        public void Success_CookingTime_Null()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.CookingTime = null;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result?.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Success_Difficulty_Null()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Difficulty = null;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result?.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Success_DishTypes_Empty()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.DishTypes.Clear();

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Error_DishTypes_Invalid()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.DishTypes.Add((MyRecipeBook.Communication.Enums.DishType)1000);

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.DISH_TYPE_NOT_SUPPORTED));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Error_Ingredients_Empty(string ingredient)
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Ingredients.Add(ingredient);

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.INGREDIENT_EMPTY));
        }

        [Fact]
        public void Error_Ingredients_Null()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Ingredients.Clear();

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.AT_LEAST_ONE_INGREDIENT));
        }

        [Fact]
        public void Error_Instructions_Null()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.Clear();

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.AT_LEAST_ONE_INSTRUCTION));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Error_Instructions_Empty(string text)
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.First().Text = text;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be(ResourceMessagesException.INSTRUCTION_EMPTY);
        }

        [Fact]
        public void Error_Instructions_Exceeds_Max_Length()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.Clear();
            request.Instructions.Add(new RequestInstructionJson
            {
                Step = 1,
                Text = new Faker().Lorem.Paragraph(2100)
            });

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be(ResourceMessagesException.INSTRUCTION_EXCEEDS_LIMIT_CHARACTERS);
        }

        [Fact]
        public void Error_Instructions_With_Same_Order()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.First().Step = request.Instructions.Last().Step;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be(ResourceMessagesException.TWO_OR_MORE_INSTRUCTIONS_SAME_ORDER);
        }

        [Fact]
        public void Error_Instructions_With_Negative_Steps()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Instructions.First().Step = -1;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be(ResourceMessagesException.NON_NEGATIVE_INSTRUCTION_STEP);
        }
    }
}
