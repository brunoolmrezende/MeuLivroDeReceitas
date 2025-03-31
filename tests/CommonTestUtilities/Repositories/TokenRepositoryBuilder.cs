using Moq;
using MyRecipeBook.Domain.Repositories.Token;

namespace CommonTestUtilities.Repositories
{
    public class TokenRepositoryBuilder
    {
        private readonly Mock<ITokenRepository> _mock;

        public TokenRepositoryBuilder() => _mock = new Mock<ITokenRepository>();


        public ITokenRepository Build() => _mock.Object;
        
            
        
    }
}
