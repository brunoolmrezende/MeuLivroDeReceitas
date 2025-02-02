using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Services.OpenAI;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Generate
{
    public class GenerateRecipeUseCase : IGenerateRecipeUseCase
    {
        private readonly IGenerateRecipeAI _generator;

        public GenerateRecipeUseCase(IGenerateRecipeAI generator)
        {
            _generator = generator;
        }

        public async Task<ResponseGeneratedRecipeJson> Execute(RequestGenerateRecipeJson request)
        {
            Validate(request);

            var response = await _generator.Generate(request.Ingredients);

            return new ResponseGeneratedRecipeJson
            {
                Title = response.Title,
                Ingredients = response.Ingredients,
                Instructions = response.Instructions.Select(x => new ResponseGeneratedInstructionJson
                {
                    Step = x.Step,
                    Text = x.Text,
                }).ToList(),
                Difficulty = Communication.Enums.Difficulty.Low,
                CookingTime = (Communication.Enums.CookingTime)response.CookingTime
            };
        }

        private void Validate(RequestGenerateRecipeJson request)
        {
            var validator = new GenerateRecipeValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errors);
            }
        }
    }
}
