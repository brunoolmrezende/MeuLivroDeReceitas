using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Delete.Delete;

namespace UseCases.Test.User.Delete.Delete
{
    public class DeleteUserAccountUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            (var user, _) = UserBuilder.Build();

            var useCase = CreateUseCase();

            var act = async () => await useCase.Execute(user.UserIdentifier);

            await act.Should().NotThrowAsync();
        }

        private DeleteUserAccountUseCase CreateUseCase()
        {
            var unitOfWork = UnitOfWorkBuilder.Build();
            var storageService = new BlobStorageServiceBuilder().Build();
            var userDeleteOnlyRepository = UserDeleteOnlyRepositoryBuilder.Build();

            return new DeleteUserAccountUseCase(storageService, userDeleteOnlyRepository, unitOfWork);
        }
    }
}
