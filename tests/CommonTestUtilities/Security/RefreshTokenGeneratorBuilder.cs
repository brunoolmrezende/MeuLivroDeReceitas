using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure.Security.Tokens.Refresh;

namespace CommonTestUtilities.Security
{
    public class RefreshTokenGeneratorBuilder
    {
        public static IRefreshTokenGenerator Build()
        {
            return new RefreshTokenGenerator();
        }
    }
}
