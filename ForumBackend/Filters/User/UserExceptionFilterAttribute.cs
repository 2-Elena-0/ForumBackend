using ForumBackend.Exceptions.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ForumBackend.Filters.User;

public class UserExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is UserNotFoundException)
        {
            context.Result = new BadRequestObjectResult(new { Errorb = context.Exception.Message });
            context.ExceptionHandled = true;
        }
    }
}