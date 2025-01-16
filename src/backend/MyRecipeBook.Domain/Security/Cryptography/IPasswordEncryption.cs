namespace MyRecipeBook.Domain.Security.Cryptography
{
    public interface IPasswordEncryption
    {
        public string Encrypt(string password);
    }
}
