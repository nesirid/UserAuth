using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SharedLibrary.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeRoleAttribute(params UserRole[] roles) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (context.ActionDescriptor.EndpointMetadata
            .Any(meta => meta is AllowAnonymousAttribute))
            return;

        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var roleValue = user.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(roleValue) ||
            !Enum.TryParse<UserRole>(roleValue, out var role) ||
            !roles.Contains(role))
        {
            context.Result = new ForbidResult();
        }
    }
}
