namespace MyRecipeBook.Exceptions.ExceptionBase
{
    public class NotFoundException : MyRecipeBookException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
