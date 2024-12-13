using MyRecipeBook.Application.Services.Cryptography;

namespace CommonTestUtilities.Cryptography
{
    public class PasswordEncryptionBuilder
    {
        public static PasswordEncryption Build() => new PasswordEncryption("abcd1234");
    }
}
