using ForumBackend.Exceptions.Comment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ForumBackend.Filters.Comment;

public class CommentExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is CommentNotFoundException)
        {
            context.Result = new NotFoundObjectResult(new {Error = context.Exception.Message});
            context.ExceptionHandled = true;
        }
    }
}