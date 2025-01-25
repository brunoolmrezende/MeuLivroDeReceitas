using CommonTestUtilities.AutoMapper;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace UseCases.Test.Recipe.Register
{
    public class RegisterRecipeUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            (var user, var password) = UserBuilder.Build();

            var request = RequestRecipeJsonBuilder.Build();

            var useCase = CreateUseCase(user);

            var result = await useCase.Execute(request);

            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrWhiteSpace();
            result.Title.Should().Be(request.Title);
        }

        [Fact]
        public async Task Error_Title_Empty()
        {
            (var user, var password) = UserBuilder.Build();

            var request = RequestRecipeJsonBuilder.Build();
            request.Title = string.Empty;

            var useCase = CreateUseCase(user);

            Func<Task> act = () => useCase.Execute(request);

            await act.Should().ThrowAsync<ErrorOnValidationException>()
                .Where(e => e.GetErrorMessages().Count == 1 &&
                    e.GetErrorMessages().Contains(ResourceMessagesException.RECIPE_TITLE_EMPTY));
        }

        private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var mapper = MapperBuilder.Build();
            var recipeWriteOnlyRepository = RecipeWriteOnlyRepositoryBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();

            return new RegisterRecipeUseCase(loggedUser, mapper, unitOfWork, recipeWriteOnlyRepository);
        }
    }
}
