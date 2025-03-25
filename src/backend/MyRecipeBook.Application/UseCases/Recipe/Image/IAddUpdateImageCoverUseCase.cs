using Microsoft.AspNetCore.Http;

namespace MyRecipeBook.Application.UseCases.Recipe.Image
{
    public interface IAddUpdateImageCoverUseCase
    {
        public Task Execute(long id, IFormFile file);
    }
}
