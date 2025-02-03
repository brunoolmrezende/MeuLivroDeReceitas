using Bogus;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Enums;

namespace CommonTestUtilities.Dtos
{
    public class GeneratedRecipeDtoBuilder
    {
        public static GeneratedRecipeDto Build()
        {
            return new Faker<GeneratedRecipeDto>()
                .RuleFor(recipe => recipe.Title, faker => faker.Lorem.Word())
                .RuleFor(recipe => recipe.CookingTime, faker => faker.PickRandom<CookingTime>())
                .RuleFor(recipe => recipe.Ingredients, faker => faker.Make(1, () => faker.Commerce.ProductName()))
                .RuleFor(recipe => recipe.Instructions, faker => faker.Make(1, () => new GeneratedInstructionDto
                {
                    Step = 1,
                    Text = faker.Lorem.Paragraph()
                }));
        }
    }
}
