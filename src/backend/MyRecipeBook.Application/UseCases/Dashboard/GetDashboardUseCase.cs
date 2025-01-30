using AutoMapper;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;

namespace MyRecipeBook.Application.UseCases.Dashboard
{
    public class GetDashboardUseCase : IGetDashboardUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IRecipeReadOnlyRepository _recipeReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetDashboardUseCase(
            ILoggedUser loggedUser,
            IRecipeReadOnlyRepository recipeReadOnlyRepository,
            IMapper mapper)
        {
            _loggedUser = loggedUser;
            _recipeReadOnlyRepository = recipeReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<ResponseRecipesJson> Execute()
        {
            var loggedUser = await _loggedUser.User();

            var recipes = await _recipeReadOnlyRepository.GetForDashboard(loggedUser);

            return new ResponseRecipesJson
            {
                Recipes = _mapper.Map<IList<ResponseShortRecipeJson>>(recipes)
            };
        }
    }
}
