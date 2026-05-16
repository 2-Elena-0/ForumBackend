using ForumBackend.Exceptions.Role;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ForumBackend.Filters.Role;

public class RoleExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is RoleNotFoundException)
        {
            context.Result = new NotFoundObjectResult(new {Error = context.Exception.Message});
            context.ExceptionHandled = true;
        }

        if (context.Exception is DeleteBaseRoleException)
        {
            context.Result = new BadRequestObjectResult(new {Error = context.Exception.Message});
            context.ExceptionHandled = true;
        }
    }
}