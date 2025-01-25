using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions.ExceptionBase;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.API.Filters
{
    public class AuthenticatedUserFilter : IAsyncAuthorizationFilter
    {
        private readonly IAccessTokenValidator _accessTokenValidator;
        private readonly IUserReadOnlyRepository _readOnlyRepository;

        public AuthenticatedUserFilter(
            IAccessTokenValidator accessTokenValidator,
            IUserReadOnlyRepository readOnlyRepository)
        {
            _accessTokenValidator = accessTokenValidator;
            _readOnlyRepository = readOnlyRepository;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var token = TokenOnRequest(context);

                var userIdentifier = _accessTokenValidator.ValidateAndGetUserIdentifier(token);

                var userExists = await _readOnlyRepository.ExistActiveUserWithUserIdentifier(userIdentifier);

                if (!userExists)
                    throw new UnauthorizedException(ResourceMessagesException.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE);
            }
            catch (MyRecipeBookException myRecipeBookException)
            {
                context.Result = new ObjectResult(new ResponseErrorJson(myRecipeBookException.GetErrorMessages()));
                context.HttpContext.Response.StatusCode = (int)myRecipeBookException.GetHttpStatusCode();

            }
            catch (SecurityTokenExpiredException)
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson("TokenIsExpired.")
                {
                    TokenIsExpired = true,
                });
            }
            catch
            {
                context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesException.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE));
            }
        }

        private static string TokenOnRequest(AuthorizationFilterContext context)
        {
            var authentication = context.HttpContext.Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authentication))
                throw new UnauthorizedException(ResourceMessagesException.NO_TOKEN); 

            return authentication["Bearer ".Length..].Trim();
        }
    }
}
