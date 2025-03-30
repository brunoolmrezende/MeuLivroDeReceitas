using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Security;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Login.External;

namespace UseCases.Test.Login.External
{
    public class ExternalLoginUseCaseTest
    {
        [Fact]
        public async Task Success_User_Dont_Exist()
        {
            (var user, _) = UserBuilder.Build();

            var useCase = CreateUseCase();

            var result = await useCase.Execute(user.Name, user.Email);

            result.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Success_User_Exist()
        {
            (var user, _) = UserBuilder.Build();

            var useCase = CreateUseCase(user);

            var result = await useCase.Execute(user.Name, user.Email);

            result.Should().NotBeNullOrWhiteSpace();
        }

        private ExternalLoginUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User? user = null)
        {
            var userWriteOnlyRepository = UserWriteOnlyRepositoryBuilder.Build();
            var userReadOnlyRepository = new UserReadOnlyRepositoryBuilder();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var tokenGenerator = JwtTokenGeneratorBuilder.Build();

            if (user is not null)
            {
                userReadOnlyRepository.GetByEmail(user);
            }

            return new ExternalLoginUseCase(userReadOnlyRepository.Build(), userWriteOnlyRepository, unitOfWork, tokenGenerator);
        }
    }
}
