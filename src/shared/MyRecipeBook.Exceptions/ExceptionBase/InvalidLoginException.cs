namespace MyRecipeBook.Exceptions.ExceptionBase
{
    public class InvalidLoginException : MyRecipeBookException
    {
        public InvalidLoginException() : base(ResourceMessagesException.INVALID_EMAIL_OR_PASSWORD) { }
    }
}
