using AutoMapper;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter
{
    public class FilterRecipeUseCase : IFilterRecipeUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IRecipeReadOnlyRepository _recipeReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IBlobStorageService _blobStorageService;

        public FilterRecipeUseCase(
            ILoggedUser loggedUser, 
            IRecipeReadOnlyRepository recipeReadOnlyRepository, 
            IMapper mapper,
            IBlobStorageService blobStorageService)
        {
            _loggedUser = loggedUser;
            _recipeReadOnlyRepository = recipeReadOnlyRepository;
            _mapper = mapper;
            _blobStorageService = blobStorageService;
        }

        public async Task<ResponseRecipesJson> Execute(RequestFilterRecipeJson request)
        {
            Validate(request);

            var loggedUser = await _loggedUser.User();

            var filters = new FilterRecipeDto
            {
                RecipeTitle_Ingredient = request.RecipeTitle_Ingredient,
                CookingTimes = request.CookingTimes.Distinct().Select(cookingTime => (Domain.Enums.CookingTime)cookingTime).ToList(),
                Difficulties = request.Difficulties.Distinct().Select(difficulty => (Domain.Enums.Difficulty)difficulty).ToList(),
                DishTypes = request.DishTypes.Distinct().Select(dishType => (Domain.Enums.DishType)dishType).ToList(),
            };

            var recipes = await _recipeReadOnlyRepository.Filter(loggedUser, filters);

            return new ResponseRecipesJson
            {
                Recipes = await recipes.MapToShortRecipeJson(loggedUser, _blobStorageService, _mapper)
            };
        }

        private static void Validate(RequestFilterRecipeJson request)
        {
            var validator = new FilterRecipeValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
                throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
        }
    }
}
