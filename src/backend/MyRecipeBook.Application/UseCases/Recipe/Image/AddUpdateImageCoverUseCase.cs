using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Image
{
    public class AddUpdateImageCoverUseCase : IAddUpdateImageCoverUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IRecipeUpdateOnlyRepository _recipeUpdateOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobStorageService _blobStorageService;

        public AddUpdateImageCoverUseCase(
            ILoggedUser loggedUser,
            IRecipeUpdateOnlyRepository recipeUpdateOnlyRepository,
            IUnitOfWork unitOfWork,
            IBlobStorageService blobStorageService)
        {
            _loggedUser = loggedUser;
            _recipeUpdateOnlyRepository = recipeUpdateOnlyRepository;
            _unitOfWork = unitOfWork;
            _blobStorageService = blobStorageService;
        }

        public async Task Execute(long id, IFormFile file)
        {
            var loggedUser = await _loggedUser.User();

            var recipe = await _recipeUpdateOnlyRepository.GetById(loggedUser, id);

            if (recipe is null)
            {
                throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);
            }

            var fileStream = file.OpenReadStream();

            (var isValidImage, var extension) = fileStream.ValidateAndGetImageExtension();

            if (!isValidImage)
            {
                throw new ErrorOnValidationException([ResourceMessagesException.ONLY_IMAGES_ACCEPTED]);
            }

            if (string.IsNullOrWhiteSpace(recipe.ImageIdentifier))
            {
                recipe.ImageIdentifier = $"{Guid.NewGuid()}{extension}";

                _recipeUpdateOnlyRepository.Update(recipe);

                await _unitOfWork.Commit();
            }

            await _blobStorageService.Upload(loggedUser, fileStream, recipe.ImageIdentifier);
        }
    }
}
