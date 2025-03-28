using AutoMapper;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById
{
    public class GetRecipeByIdUseCase : IGetRecipeByIdUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IRecipeReadOnlyRepository _recipeReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IBlobStorageService _blobStorageService;

        public GetRecipeByIdUseCase(
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

        public async Task<ResponseRecipeJson> Execute(long recipeId)
        {
            var loggedUser = await _loggedUser.User();

            var recipe = await _recipeReadOnlyRepository.GetById(loggedUser, recipeId);

            if (recipe == null)
            {
                throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);
            }

            var response =  _mapper.Map<ResponseRecipeJson>(recipe);

            if (!string.IsNullOrWhiteSpace(recipe.ImageIdentifier))
            {
                var url = await _blobStorageService.GetFileUrl(loggedUser, recipe.ImageIdentifier);

                response.ImageUrl = url;
            }

            return response;
        }
    }
}
