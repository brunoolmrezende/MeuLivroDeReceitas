using AutoMapper;
using FluentValidation.Results;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.User.Register
{
    public class RegisterUserUseCase(
        IUserReadOnlyRepository readOnlyRepository, 
        IUserWriteOnlyRepository writeOnlyRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IPasswordEncryption passwordEncryption,
        IAccessTokenGenerator accessTokenGenerator
        ) : IRegisterUserUseCase
    {
        private readonly IUserReadOnlyRepository _readOnlyRepository = readOnlyRepository;
        private readonly IUserWriteOnlyRepository _writeOnlyRepository = writeOnlyRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPasswordEncryption _passwordEncryption = passwordEncryption;
        private readonly IAccessTokenGenerator _accessTokenGenerator = accessTokenGenerator;

        public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
        {
            await Validate(request);

            var user = _mapper.Map<Domain.Entities.User>(request);

            user.Password = _passwordEncryption.Encrypt(request.Password);
            user.UserIdentifier = Guid.NewGuid();

            await _writeOnlyRepository.Add(user);

            await _unitOfWork.Commit();

            return new ResponseRegisteredUserJson 
            { 
                Name = request.Name,
                Tokens = new ResponseTokensJson
                {
                    AccessToken = _accessTokenGenerator.Generate(user.UserIdentifier),
                }
            };
        }

        private async Task Validate(RequestRegisterUserJson request)
        {
            var validator = new RegisterUserValidator();

            var result = validator.Validate(request);

            var emailAlreadyRegistered = await _readOnlyRepository.ExistActiveUserWithEmail(request.Email);

            if (emailAlreadyRegistered)
                result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.EMAIL_ALREADY_REGISTERED));

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => e.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errors);
            }
        }
    }
}
