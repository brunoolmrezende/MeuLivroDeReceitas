using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionBase
{
    public class ErrorOnValidationException : MyRecipeBookException
    {
        private readonly IList<string> _errorMessages;

        public ErrorOnValidationException(IList<string> errors) : base(string.Empty)
        {
            _errorMessages = errors;
        }

        public override IList<string> GetErrorMessages() => _errorMessages;

        public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.BadRequest;
    }
}
