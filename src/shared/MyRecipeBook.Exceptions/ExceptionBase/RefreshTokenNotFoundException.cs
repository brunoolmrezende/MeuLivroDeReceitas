using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionBase
{
    public class RefreshTokenNotFoundException : MyRecipeBookException
    {
        public RefreshTokenNotFoundException() : base(ResourceMessagesException.EXPIRED_SESSION)
        {
        }

        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.Unauthorized;
    }
}
