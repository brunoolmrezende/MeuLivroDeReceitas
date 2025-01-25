using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionBase
{
    public class NotFoundException : MyRecipeBookException
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public override IList<string> GetErrorMessages() => [Message];

        public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.NotFound;
    }
}
