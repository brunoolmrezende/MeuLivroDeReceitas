using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Change_Password;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;

namespace UseCases.Test.User.Change_Password
{
    public class ChangePasswordUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            (var user, var password) = UserBuilder.Build();

            var request = RequestChangePasswordJsonBuilder.Build();
            request.Password = password;

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.Execute(request);
            
            await act.Should().NotThrowAsync();

            var passwordEncrypter = PasswordEncryptionBuilder.Build();

            user.Password.Should().Be(passwordEncrypter.Encrypt(request.NewPassword));
        }

        [Fact]
        public async Task Error_Current_Password_Invalid()
        {
            (var user, _) = UserBuilder.Build();

            var request = RequestChangePasswordJsonBuilder.Build();

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.Execute(request);

            await act.Should().ThrowAsync<ErrorOnValidationException>()
                .Where(e => e.GetErrorMessages().Count == 1 &&
                    e.GetErrorMessages().Contains(ResourceMessagesException.PASSWORD_DIFFERENT_CURRENT_PASSWORD));

            var passwordEncrypter = PasswordEncryptionBuilder.Build();

            user.Password.Should().NotBe(passwordEncrypter.Encrypt(request.NewPassword));
        }

        [Fact]
        public async Task Error_New_Password_Empty()
        {
            (var user, var password) = UserBuilder.Build();

            var request = new RequestChangePasswordJson
            {
                Password = password,
                NewPassword = string.Empty
            };

            var useCase = CreateUseCase(user);

            Func<Task> act = async () => await useCase.Execute(request);

            await act.Should().ThrowAsync<ErrorOnValidationException>()
                .Where(e => e.GetErrorMessages().Count == 1 &&
                    e.GetErrorMessages().Contains(ResourceMessagesException.EMPTY_PASSWORD));

            var passwordEncrypter = PasswordEncryptionBuilder.Build();

            user.Password.Should().Be(passwordEncrypter.Encrypt(password));
        }

        private static ChangePasswordUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
        {
            var loggedUser = LoggedUserBuilder.Build(user);
            var updateOnlyRepository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var passwordEncryption = PasswordEncryptionBuilder.Build();

            return new ChangePasswordUseCase(loggedUser, updateOnlyRepository, unitOfWork, passwordEncryption);
        }
    }

}
