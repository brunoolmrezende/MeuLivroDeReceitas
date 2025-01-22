using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Domain.Dtos
{
    public record FilterRecipeDto
    {
        public string? RecipeTitle_Ingredient { get; init; } = string.Empty;
        public IList<CookingTime> CookingTimes { get; init; } = [];
        public IList<Difficulty> Difficulties { get; init; } = [];
        public IList<DishType> DishTypes { get; init; } = [];
    }
}
