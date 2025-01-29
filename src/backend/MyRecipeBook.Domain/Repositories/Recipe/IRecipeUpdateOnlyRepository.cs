
namespace MyRecipeBook.Domain.Repositories.Recipe
{
    public interface IRecipeUpdateOnlyRepository
    {
        public void Update(Domain.Entities.Recipe recipe);

        public Task<Domain.Entities.Recipe?> GetById(Domain.Entities.User user, long recipeId);
    }
}
