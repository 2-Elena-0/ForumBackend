using ForumBackend.Exceptions.Topic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ForumBackend.Filters.Topic;

public class TopicExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is TopicNotFoundException)
        {
            context.Result = new NotFoundObjectResult(new {Error = context.Exception.Message});
            context.ExceptionHandled = true;
        }
    }
}