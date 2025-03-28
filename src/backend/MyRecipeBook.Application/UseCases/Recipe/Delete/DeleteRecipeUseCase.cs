
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Delete
{
    public class DeleteRecipeUseCase : IDeleteRecipeUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IRecipeReadOnlyRepository _recipeReadOnlyRepository;
        private readonly IRecipeWriteOnlyRepository _recipeWriteOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobStorageService _blobStorageService;

        public DeleteRecipeUseCase(
            ILoggedUser loggedUser,
            IRecipeWriteOnlyRepository recipeWriteOnlyRepository,
            IRecipeReadOnlyRepository recipeReadOnlyRepository,
            IUnitOfWork unitOfWork,
            IBlobStorageService blobStorageService)
        {
            _loggedUser = loggedUser;
            _recipeReadOnlyRepository = recipeReadOnlyRepository;
            _recipeWriteOnlyRepository = recipeWriteOnlyRepository;
            _unitOfWork = unitOfWork;
            _blobStorageService = blobStorageService;
        }

        public async Task Execute(long recipeId)
        {
            var loggedUser = await _loggedUser.User();

            var recipe = await _recipeReadOnlyRepository.GetById(loggedUser, recipeId) ?? throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

            if (!string.IsNullOrWhiteSpace(recipe.ImageIdentifier))
            {
                await _blobStorageService.Delete(loggedUser, recipe.ImageIdentifier);
            }

            await _recipeWriteOnlyRepository.Delete(recipe.Id);

            await _unitOfWork.Commit();
        }
    }
}
