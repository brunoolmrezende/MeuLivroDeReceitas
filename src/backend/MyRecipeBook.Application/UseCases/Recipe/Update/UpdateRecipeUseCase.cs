using AutoMapper;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Update
{
    public class UpdateRecipeUseCase : IUpdateRecipeUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IRecipeUpdateOnlyRepository _recipeUpdateOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateRecipeUseCase(
            ILoggedUser loggedUser,
            IRecipeUpdateOnlyRepository recipeUpdateOnlyRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _loggedUser = loggedUser;
            _recipeUpdateOnlyRepository = recipeUpdateOnlyRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Execute(long recipeId, RequestRecipeJson request)
        {
            Validate(request);

            var loggedUser = await _loggedUser.User();

            var recipe = await _recipeUpdateOnlyRepository.GetById(loggedUser, recipeId) 
                        ?? throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

            recipe.Instructions.Clear();
            recipe.Ingredients.Clear();
            recipe.DishTypes.Clear();

            _mapper.Map(request, recipe);

            var instructions = request.Instructions.OrderBy(i => i.Step).ToList();
            for (int index = 0; index < instructions.Count; index++)
                instructions.ElementAt(index).Step = index + 1;

            recipe.Instructions = _mapper.Map<IList<Domain.Entities.Instruction>>(instructions);

            _recipeUpdateOnlyRepository.Update(recipe);

            await _unitOfWork.Commit();
        }

        private void Validate(RequestRecipeJson request)
        {
            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
                throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).Distinct().ToList());
        }
    }
}
