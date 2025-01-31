using CommonTestUtilities.AutoMapper;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Dashboard;

namespace UseCases.Test.Dashboard
{
    public class GetDashboardUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            (var user, _) = UserBuilder.Build();

            var recipes = RecipeBuilder.Collection(user, 6);

            var useCase = CreateUseCase(user, recipes);

            var result = await useCase.Execute();

            result.Should().NotBeNull();
            result.Recipes.Should()
                .HaveCountGreaterThan(0)
                .And.OnlyHaveUniqueItems(recipe => recipe.Id)
                .And.AllSatisfy(recipe =>
                {
                    recipe.Id.Should().NotBeNullOrWhiteSpace();
                    recipe.Title.Should().NotBeNullOrWhiteSpace();
                    recipe.AmountIngredients.Should().BeGreaterThan(0);
                });

        }

        private GetDashboardUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, IList<MyRecipeBook.Domain.Entities.Recipe> recipes)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var recipeReadOnlyRepository = new RecipeReadOnlyRepositoryBuilder().GetForDashboard(user, recipes).Build();
            var mapper = MapperBuilder.Build();

            return new GetDashboardUseCase(loggedUser, recipeReadOnlyRepository, mapper);
        }
    }
}
