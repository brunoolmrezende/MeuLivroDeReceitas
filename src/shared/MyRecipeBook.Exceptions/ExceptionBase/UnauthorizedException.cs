using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionBase
{
    public class UnauthorizedException : MyRecipeBookException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }

        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.Unauthorized;
    }
}
