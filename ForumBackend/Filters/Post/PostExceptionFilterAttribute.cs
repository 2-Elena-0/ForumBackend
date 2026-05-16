using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ForumBackend.Filters.Post;

public class PostExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is PostNotFoundException)
        {
            context.Result = new NotFoundObjectResult(new {Error = context.Exception.Message});
            context.ExceptionHandled = true;
        }
    }
}