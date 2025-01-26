
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
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

        public DeleteRecipeUseCase(
            ILoggedUser loggedUser,
            IRecipeWriteOnlyRepository recipeWriteOnlyRepository,
            IRecipeReadOnlyRepository recipeReadOnlyRepository,
            IUnitOfWork unitOfWork)
        {
            _loggedUser = loggedUser;
            _recipeReadOnlyRepository = recipeReadOnlyRepository;
            _recipeWriteOnlyRepository = recipeWriteOnlyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(long recipeId)
        {
            var loggedUser = await _loggedUser.User();

            var recipe = await _recipeReadOnlyRepository.GetById(loggedUser, recipeId) ?? throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

            await _recipeWriteOnlyRepository.Delete(recipe.Id);

            await _unitOfWork.Commit();
        }
    }
}
