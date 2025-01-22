using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories
{
    public class RecipeRepository(MyRecipeBookDbContext dbContext) : IRecipeWriteOnlyRepository, IRecipeReadOnlyRepository
    {
        private readonly MyRecipeBookDbContext _dbContext = dbContext;

        public async Task Add(Recipe recipe) => await _dbContext.AddAsync(recipe);

        public async Task<IList<Recipe>> Filter(User user, FilterRecipeDto filters)
        {
            var query = _dbContext
                .Recipes
                .AsNoTracking()
                .Include(recipe => recipe.Ingredients)
                .Where(recipe => recipe.Active && recipe.UserId.Equals(user.Id));

            if (!string.IsNullOrWhiteSpace(filters.RecipeTitle_Ingredient))
            {
                query = query.Where(
                    recipe => recipe.Title.Contains(filters.RecipeTitle_Ingredient)
                    || recipe.Ingredients.Any(ingredient => ingredient.Item.Contains(filters.RecipeTitle_Ingredient)));
            }

            if (filters.CookingTimes.Any())
            {
                query = query.Where(recipe => recipe.CookingTime.HasValue && filters.CookingTimes.Contains(recipe.CookingTime.Value));
            }

            if (filters.Difficulties.Any())
            {
                query = query.Where(recipe => recipe.Difficulty.HasValue && filters.Difficulties.Contains(recipe.Difficulty.Value));
            }

            if (filters.DishTypes.Any())
            {
                query = query.Where(recipe => recipe.DishTypes.Any(DishType => filters.DishTypes.Contains(DishType.Type)));
            }

            return await query.ToListAsync();
        }
    }
}
