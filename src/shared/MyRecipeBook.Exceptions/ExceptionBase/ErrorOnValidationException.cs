namespace MyRecipeBook.Exceptions.ExceptionBase
{
    public class ErrorOnValidationException : MyRecipeBookException
    {
        public IList<string> ErrorMessages { get; set; }

        public ErrorOnValidationException(IList<string> errors) : base(string.Empty)
        {
            ErrorMessages = errors;
        }
    }
}
