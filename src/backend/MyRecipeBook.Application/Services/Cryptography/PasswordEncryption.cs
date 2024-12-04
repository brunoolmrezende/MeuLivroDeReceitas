using System.Security.Cryptography;
using System.Text;

namespace MyRecipeBook.Application.Services.Cryptography
{
    public class PasswordEncryption(string additionalKey)
    {
        private readonly string _additionalKey = additionalKey;
        public string Encrypt(string password)
        {
            var salt = _additionalKey;

            var newPassword = $"{password}{salt}";

            var bytes = Encoding.UTF8.GetBytes(newPassword);

            var hashBytes = SHA512.HashData(bytes);

            return StringBytes(hashBytes);
        }

        private static string StringBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2")); // Converte para hexadecimal
            }

            return sb.ToString();
        }
    }
}
