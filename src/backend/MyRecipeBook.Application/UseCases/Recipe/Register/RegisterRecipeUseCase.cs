using AutoMapper;
using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Register
{
    public class RegisterRecipeUseCase : IRegisterRecipeUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRecipeWriteOnlyRepository _recipeWriteOnlyRepository;
        private readonly IBlobStorageService _blobStorageService;

        public RegisterRecipeUseCase(
            ILoggedUser loggedUser,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IRecipeWriteOnlyRepository recipeWriteOnlyRepository,
            IBlobStorageService blobStorageService)
        {
            _loggedUser = loggedUser;
            _mapper = mapper;
            _recipeWriteOnlyRepository = recipeWriteOnlyRepository;
            _unitOfWork = unitOfWork;
            _blobStorageService = blobStorageService;
        }

        public async Task<ResponseRegisteredRecipeJson> Execute(RequestRegisterRecipeFormData request)
        {
            Validate(request);

            var loggedUser = await _loggedUser.User();

            var recipe = _mapper.Map<Domain.Entities.Recipe>(request);
            recipe.UserId = loggedUser.Id;

            var instructions = request.Instructions.OrderBy(i => i.Step).ToList();
            for (var index = 0; index < instructions.Count; index++)
                instructions[index].Step = index + 1;

            recipe.Instructions = _mapper.Map<IList<Instruction>>(instructions);

            if (request.Image is not null)
            {
                var fileStream = request.Image.OpenReadStream();

                (var isValidImage, var extension) = fileStream.ValidateAndGetImageExtension();

                if (!isValidImage)
                {
                    throw new ErrorOnValidationException([ResourceMessagesException.ONLY_IMAGES_ACCEPTED]);
                }

                recipe.ImageIdentifier = $"{Guid.NewGuid()}{extension}";

                await _blobStorageService.Upload(loggedUser,fileStream, recipe.ImageIdentifier);
            }

            await _recipeWriteOnlyRepository.Add(recipe);

            await _unitOfWork.Commit();

            return _mapper.Map<ResponseRegisteredRecipeJson>(recipe);
        }

        private static void Validate(RequestRecipeJson request)
        {
            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
                throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).Distinct().ToList());
        }
    }
}
