using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Register
{
    public class RegisterRecipeUseCase : IRegisterRecipeUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRecipeWriteOnlyRepository _recipeWriteOnlyRepository;

        public RegisterRecipeUseCase(
            ILoggedUser loggedUser,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IRecipeWriteOnlyRepository recipeWriteOnlyRepository)
        {
            _loggedUser = loggedUser;
            _mapper = mapper;
            _recipeWriteOnlyRepository = recipeWriteOnlyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseRegisteredRecipeJson> Execute(RequestRecipeJson request)
        {
            Validate(request);

            var loggedUser = await _loggedUser.User();

            var recipe = _mapper.Map<Domain.Entities.Recipe>(request);
            recipe.UserId = loggedUser.Id;

            var instructions = request.Instructions.OrderBy(i => i.Step).ToList();
            for (var i = 0; i < instructions.Count; i++)
                instructions.ElementAt(i).Step = i + 1;

            recipe.Instructions = _mapper.Map<IList<Instruction>>(instructions);

            await _recipeWriteOnlyRepository.Add(recipe);

            await _unitOfWork.Commit();

            return _mapper.Map<ResponseRegisteredRecipeJson>(recipe);
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
