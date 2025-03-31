using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionBase
{
    public class RefreshTokenExpiredException : MyRecipeBookException
    {
        public RefreshTokenExpiredException() : base(ResourceMessagesException.INVALID_SESSION)
        {
        }

        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.Forbidden;
    }
}
