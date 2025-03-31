namespace MyRecipeBook.Domain.Repositories.Token
{
    public interface ITokenRepository
    {
        Task<Entities.RefreshToken?> GetToken(string refreshToken);
        Task SaveNewRefreshToken(Entities.RefreshToken refreshToken);
    }
}
