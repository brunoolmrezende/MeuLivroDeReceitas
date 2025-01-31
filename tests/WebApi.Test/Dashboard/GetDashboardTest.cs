using CommonTestUtilities.Security;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace WebApi.Test.Dashboard
{
    public class GetDashboardTest : MyRecipeBookClassFixture
    {
        private readonly string _endpoint = "dashboard";
        private readonly Guid _userIdentifier;

        public GetDashboardTest(CustomWebApplicationFactory factory) : base(factory)
        {
            _userIdentifier = factory.GetUserIdentifier();
        }

        public async Task Success()
        {
            var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

            var response = await DoGet(_endpoint, token);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await using var responseBody = await response.Content.ReadAsStreamAsync();

            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("recipes").GetArrayLength().Should().BeGreaterThan(0);
        }
    }
}
