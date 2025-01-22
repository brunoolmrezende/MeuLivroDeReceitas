using MyRecipeBook.Domain.Dtos;

namespace MyRecipeBook.Domain.Repositories.Recipe
{
    public interface IRecipeReadOnlyRepository
    {
        Task<IList<Entities.Recipe>> Filter(Domain.Entities.User user, FilterRecipeDto filters);
    }
}
