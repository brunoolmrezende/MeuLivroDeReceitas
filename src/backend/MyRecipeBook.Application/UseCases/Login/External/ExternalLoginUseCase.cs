using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.Application.UseCases.Login.External
{
    public class ExternalLoginUseCase : IExternalLoginUseCase
    {
        private readonly IUserReadOnlyRepository _readOnlyRepository;
        private readonly IUserWriteOnlyRepository _writeOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessTokenGenerator _tokenGenerator;

        public ExternalLoginUseCase(
            IUserReadOnlyRepository readOnlyRepository,
            IUserWriteOnlyRepository writeOnlyRepository,
            IUnitOfWork unitOfWork,
            IAccessTokenGenerator tokenGenerator)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _unitOfWork = unitOfWork;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<string> Execute(string name, string email)
        {
            var user = await _readOnlyRepository.GetByEmail(email);

            if (user is null)
            {
                user = new Domain.Entities.User
                {
                    Name = name,
                    Email = email,
                    Password = "-",
                    UserIdentifier = Guid.NewGuid(),
                };

                await _writeOnlyRepository.Add(user);
                await _unitOfWork.Commit();
            }

            return _tokenGenerator.Generate(user.UserIdentifier);
        }
    }
}
