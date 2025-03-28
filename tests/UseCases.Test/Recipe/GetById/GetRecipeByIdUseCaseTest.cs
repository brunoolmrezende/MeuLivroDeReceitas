using CommonTestUtilities.AutoMapper;
using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace UseCases.Test.Recipe.GetById
{
    public class GetRecipeByIdUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            (var user, var password) = UserBuilder.Build();

            var recipe = RecipeBuilder.Build(user);

            var useCase = CreateUseCase(user, recipe);

            var result = await useCase.Execute(recipe.Id);

            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrWhiteSpace();
            result.Title.Should().Be(recipe.Title);
            result.ImageUrl.Should().NotBeNull();
        }

        [Fact]
        public async Task Error_Recipe_Not_Found()
        {
            (var user, var password) = UserBuilder.Build();

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.Execute(recipeId: 1000);

           await act.Should().ThrowAsync<NotFoundException>()
                .Where(e => e.GetErrorMessages().Count == 1 && 
                    e.GetErrorMessages().Contains(ResourceMessagesException.RECIPE_NOT_FOUND));
        }

        private static GetRecipeByIdUseCase CreateUseCase(
            MyRecipeBook.Domain.Entities.User user, 
            MyRecipeBook.Domain.Entities.Recipe? recipe = null)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var recipeReadOnlyRepository = new RecipeReadOnlyRepositoryBuilder().GetById(user, recipe).Build();
            var mapper = MapperBuilder.Build();
            var blobStorage = new BlobStorageServiceBuilder().GetFileUrl(user, recipe?.ImageIdentifier).Build();
            
            return new GetRecipeByIdUseCase(loggedUser, recipeReadOnlyRepository, mapper, blobStorage);
        }
    }
}
