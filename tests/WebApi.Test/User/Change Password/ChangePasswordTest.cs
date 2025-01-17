using CommonTestUtilities.Requests;
using CommonTestUtilities.Security;
using FluentAssertions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.Change_Password
{
    public class ChangePasswordTest : MyRecipeBookClassFixture
    {
        private readonly string _endpoint = "user/change-password";

        private readonly Guid _userIdentifier;
        private readonly string _password;
        private readonly string _email;

        public ChangePasswordTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
            _password = factory.GetPassword();
            _email = factory.GetEmail();
        }

        [Fact]
        public async Task Success()
        {
            var request = RequestChangePasswordJsonBuilder.Build();
            request.Password = _password;

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoPut(_endpoint, request, token);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var loginRequest = new RequestDoLoginJson
            {
                Email = _email,
                Password = request.NewPassword,
            };

            response = await DoPost(endpoint:"login", request: loginRequest);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [ClassData(typeof(CultureInlineDataTest))]
        public async Task Error_New_Password_Empty(string culture)
        {
            var request = new RequestChangePasswordJson
            {
                NewPassword = string.Empty,
                Password = _password,
            };

            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoPut(_endpoint, request, token, culture);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            
            using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

            var expectedMessage = ResourceMessagesException.ResourceManager.GetString("EMPTY_PASSWORD", new CultureInfo(culture));

            errors.Should().ContainSingle().Which.GetString().Should().Be(expectedMessage);
            errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));
        }
    }
}
