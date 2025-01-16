using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Exceptions;

namespace Validators.Test.User.Update
{
    public class UpdateUserValidatorTest
    {
        [Fact]
        public void Success()
        {
            var request = RequestUpdateUserJsonBuilder.Build();

            var validator = new UpdateUserValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Error_Name_Empty()
        {
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Name = string.Empty;

            var validator = new UpdateUserValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be(ResourceMessagesException.NAME_EMPTY);
        }

        [Fact]
        public void Error_Email_Empty()
        {
            var requet = RequestUpdateUserJsonBuilder.Build();
            requet.Email = string.Empty;

            var validator = new UpdateUserValidator();

            var result = validator.Validate(requet);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be(ResourceMessagesException.EMAIL_EMPTY);
        }

        [Fact]
        public void Error_Email_Invalid()
        {
            var request = RequestUpdateUserJsonBuilder.Build();
            request.Email = "email.com";

            var validator = new UpdateUserValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be(ResourceMessagesException.INVALID_EMAIL);
        }
    }
}
