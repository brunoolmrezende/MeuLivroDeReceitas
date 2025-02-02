using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Recipe.Generate
{
    public class GenerateRecipeValidator : AbstractValidator<RequestGenerateRecipeJson>
    {
        public GenerateRecipeValidator()
        {
            var maximum_number_ingredientes = MyRecipeBookRuleConstants.MAXIMUM_INGREDIENTS_GENERATE_RECIPE;

            RuleFor(request => request.Ingredients.Count).InclusiveBetween(1, maximum_number_ingredientes).WithMessage(ResourceMessagesException.INVALID_NUMBER_INGREDIENTS);

            RuleFor(request => request.Ingredients).Must(ingredients => ingredients.Count == ingredients.Distinct().Count()).WithMessage(ResourceMessagesException.DUPLICATED_INGREDIENTS_IN_LIST);

            RuleFor(request => request.Ingredients).ForEach(rule =>
            {
                rule.Custom((value, context) =>
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        context.AddFailure(string.Empty, ResourceMessagesException.INGREDIENT_EMPTY);
                        return;
                    }

                    if (value.Count(x => x == ' ') > 3 || value.Count(x => x == '/') > 1)
                    {
                        context.AddFailure(string.Empty, ResourceMessagesException.INGREDIENT_NOT_FOLLOWING_PATTERN);
                        return;
                    }
                });
            });
        }
    }
}
