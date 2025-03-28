using CommonTestUtilities.AutoMapper;
using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;
using UseCases.Test.Recipe.InlineDatas;

namespace UseCases.Test.Recipe.Register
{
    public class RegisterRecipeUseCaseTest
    {
        [Fact]
        public async Task Success_Without_Image()
        {
            (var user, var password) = UserBuilder.Build();

            var request = RequestRegisterRecipeFormDataBuilder.Build();

            var useCase = CreateUseCase(user);

            var result = await useCase.Execute(request);

            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrWhiteSpace();
            result.Title.Should().Be(request.Title);
        }

        [Theory]
        [ClassData(typeof(ImageTypesInlineData))]
        public async Task Success_With_Image(IFormFile file)
        {
            (var user, var password) = UserBuilder.Build();

            var request = RequestRegisterRecipeFormDataBuilder.Build(file);

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

            var request = RequestRegisterRecipeFormDataBuilder.Build();
            request.Title = string.Empty;

            var useCase = CreateUseCase(user);

            Func<Task> act = () => useCase.Execute(request);

            await act.Should().ThrowAsync<ErrorOnValidationException>()
                .Where(e => e.GetErrorMessages().Count == 1 &&
                    e.GetErrorMessages().Contains(ResourceMessagesException.RECIPE_TITLE_EMPTY));
        }

        [Fact]
        public async Task Error_Invalid_File()
        {
            (var user, var password) = UserBuilder.Build();

            var textfile = FormFileBuilder.Txt();

            var request = RequestRegisterRecipeFormDataBuilder.Build(textfile);

            var useCase = CreateUseCase(user);

            Func<Task> act = () => useCase.Execute(request);

            await act.Should().ThrowAsync<ErrorOnValidationException>()
                .Where(e => e.GetErrorMessages().Count == 1 &&
                    e.GetErrorMessages().Contains(ResourceMessagesException.ONLY_IMAGES_ACCEPTED));
        }

        private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var mapper = MapperBuilder.Build();
            var recipeWriteOnlyRepository = RecipeWriteOnlyRepositoryBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var blobStorage = new BlobStorageServiceBuilder().Build();

            return new RegisterRecipeUseCase(loggedUser, mapper, unitOfWork, recipeWriteOnlyRepository, blobStorage);
        }
    }
}
