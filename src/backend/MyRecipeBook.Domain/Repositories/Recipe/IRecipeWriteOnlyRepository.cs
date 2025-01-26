namespace MyRecipeBook.Domain.Repositories.Recipe
{
    public interface IRecipeWriteOnlyRepository
    {
        Task Add(Domain.Entities.Recipe recipe);
        Task Delete(long recipeId);
    }
}
