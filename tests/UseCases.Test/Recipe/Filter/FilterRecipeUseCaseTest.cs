using CommonTestUtilities.AutoMapper;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace UseCases.Test.Recipe.Filter
{
    public class FilterRecipeUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            (var user, _) = UserBuilder.Build();

            var request = RequestFilterRecipeJsonBuilder.Build();

            var recipes = RecipeBuilder.Collection(user);

            var useCase = CreateUseCase(user, recipes);

            var result = await useCase.Execute(request);

            result.Should().NotBeNull();
            result.Recipes.Should().NotBeNullOrEmpty();
            result.Recipes.Should().HaveCount(recipes.Count);
        }

        [Fact]
        public async Task Error_Invalid_CookingTime()
        {
            (var user, _) = UserBuilder.Build();

            var request = RequestFilterRecipeJsonBuilder.Build();
            request.CookingTimes.Add((MyRecipeBook.Communication.Enums.CookingTime)1000);

            var recipes = RecipeBuilder.Collection(user);

            var useCase = CreateUseCase(user, recipes);

            Func<Task> act = async () => await useCase.Execute(request);

            await act.Should().ThrowAsync<ErrorOnValidationException>()
                .Where(error => error.ErrorMessages.Count == 1 &&
                    error.ErrorMessages.Contains(ResourceMessagesException.COOKING_TIME_NOT_SUPPORTED));
        }

        private static FilterRecipeUseCase CreateUseCase(
            MyRecipeBook.Domain.Entities.User user, 
            IList<MyRecipeBook.Domain.Entities.Recipe> recipes)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var mapper = MapperBuilder.Build();
            var recipeReadOnlyRepository = new RecipeReadOnlyRepositoryBuilder().Filter(user, recipes).Build();

            return new FilterRecipeUseCase(loggedUser, recipeReadOnlyRepository, mapper);
        }
    }
}
