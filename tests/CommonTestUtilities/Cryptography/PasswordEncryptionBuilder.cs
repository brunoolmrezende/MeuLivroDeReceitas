using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Infrastructure.Security.Cryptography;

namespace CommonTestUtilities.Cryptography
{
    public class PasswordEncryptionBuilder
    {
        public static IPasswordEncryption Build() => new BCryptNet();
    }
}
