using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin
{
    public class DoLoginUseCase(
        IUserReadOnlyRepository readOnlyRepository,
        IPasswordEncryption passwordEncryption,
        IAccessTokenGenerator accessTokenGenerator) : IDoLoginUseCase
    {
        private readonly IUserReadOnlyRepository _readOnlyRepository = readOnlyRepository;
        private readonly IPasswordEncryption _passwordEncryption = passwordEncryption;
        private readonly IAccessTokenGenerator _accessTokenGenerator = accessTokenGenerator;

        public async Task<ResponseRegisteredUserJson> Execute(RequestDoLoginJson request)
        {
            var encryptedPassword = _passwordEncryption.Encrypt(request.Password);

            var user = await _readOnlyRepository.GetByEmailAndPassword(request.Email, encryptedPassword) ?? throw new InvalidLoginException();

            return new ResponseRegisteredUserJson
            {
                Name = user.Name,
                Tokens = new ResponseTokensJson
                {
                    AccessToken = _accessTokenGenerator.Generate(user.UserIdentifier),
                }
            };
        }
    }
}
