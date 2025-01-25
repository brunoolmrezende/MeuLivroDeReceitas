using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionBase;
using System;

namespace MyRecipeBook.API.Filters
{
    public class ExceptionFilters : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is MyRecipeBookException myRecipeBookException)
                HandleProjectExceptions(context, myRecipeBookException);
            else
                ThrowUnknownException(context);   
        }

        private static void HandleProjectExceptions(ExceptionContext context, MyRecipeBookException myRecipeBookException)
        {
            context.HttpContext.Response.StatusCode = (int)myRecipeBookException.GetHttpStatusCode();
            context.Result = new ObjectResult(new ResponseErrorJson(myRecipeBookException.GetErrorMessages()));
        }

        private static void ThrowUnknownException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesException.UNKNOWN_ERROR));
        }
    }
}
