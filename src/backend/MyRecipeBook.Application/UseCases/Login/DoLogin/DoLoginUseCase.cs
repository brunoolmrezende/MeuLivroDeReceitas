using MyRecipeBook.Application.Services.Cryptography;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin
{
    public class DoLoginUseCase(
        IUserReadOnlyRepository readOnlyRepository,
        PasswordEncryption passwordEncryption) : IDoLoginUseCase
    {
        private readonly IUserReadOnlyRepository _readOnlyRepository = readOnlyRepository;
        private readonly PasswordEncryption _passwordEncryption = passwordEncryption;
        public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
        {
            var encryptedPassword = _passwordEncryption.Encrypt(request.Password);

            var user = await _readOnlyRepository.GetByEmailAndPassword(request.Email, encryptedPassword) ?? throw new InvalidLoginException();

            return new ResponseRegisteredUserJson
            {
                Name = user.Name,
            };
        }
    }
}
