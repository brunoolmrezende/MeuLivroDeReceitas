using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories
{
    public class RecipeRepository(MyRecipeBookDbContext dbContext) : IRecipeWriteOnlyRepository
    {
        private readonly MyRecipeBookDbContext _dbContext = dbContext;

        public async Task Add(Recipe recipe) => await _dbContext.AddAsync(recipe);
    }
}
