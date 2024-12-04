namespace MyRecipeBook.Exceptions.ExceptionBase
{
    public class ErrorOnValidationException(IList<string> errors) : MyRecipeBookException
    {
        public IList<string> ErrorMessages { get; set; } = errors;
    }
}
