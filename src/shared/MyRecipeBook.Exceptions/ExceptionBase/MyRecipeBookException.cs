using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionBase
{
    public abstract class MyRecipeBookException : System.Exception
    {
        public MyRecipeBookException(string message) : base(message) { }

        public abstract IList<string> GetErrorMessages();
        public abstract HttpStatusCode GetHttpStatusCode();
    }
}
