namespace MyRecipeBook.Domain.Repositories.User
{
    public interface IUserReadOnlyRepository
    {
        Task<bool> ExistActiveUserWithEmail(string email);
        Task<bool> ExistActiveUserWithUserIdentifier(Guid userIdentifier);
        Task<Entities.User?> GetByEmailAndPassword(string email, string password);
    }
}
