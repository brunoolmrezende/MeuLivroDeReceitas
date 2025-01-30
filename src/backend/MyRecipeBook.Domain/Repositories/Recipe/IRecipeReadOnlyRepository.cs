using MyRecipeBook.Domain.Dtos;

namespace MyRecipeBook.Domain.Repositories.Recipe
{
    public interface IRecipeReadOnlyRepository
    {
        Task<IList<Entities.Recipe>> Filter(Domain.Entities.User user, FilterRecipeDto filters);

        Task<Entities.Recipe?> GetById(Domain.Entities.User user, long recipeId);

        Task<IList<Entities.Recipe>> GetForDashboard(Domain.Entities.User user);
    }
}
