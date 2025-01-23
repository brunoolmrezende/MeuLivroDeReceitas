using Bogus;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestFilterRecipeJsonBuilder
    {
        public static RequestFilterRecipeJson Build()
        {
            return new Faker<RequestFilterRecipeJson>()
                .RuleFor(r => r.RecipeTitle_Ingredient, faker => faker.Lorem.Word())
                .RuleFor(r => r.CookingTimes, faker => faker.Make(1, () => faker.PickRandom<CookingTime>()))
                .RuleFor(r => r.Difficulties, faker => faker.Make(1, () => faker.PickRandom<Difficulty>()))
                .RuleFor(r => r.DishTypes, faker => faker.Make(1, () => faker.PickRandom<DishType>()));     
        }
    }
}
