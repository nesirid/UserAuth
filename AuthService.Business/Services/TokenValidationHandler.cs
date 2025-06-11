using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AuthService.DAL.Contexts;

namespace AuthService.Business.Services
{
    //public class TokenValidationHandler
    //{
    //    public static Task OnTokenValidated(TokenValidatedContext context)
    //    {
    //        var tokenString = context.Request.Headers["Authorization"]
    //            .ToString().Replace("Bearer ", "");

    //        var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

    //        return CheckBlacklist(tokenString, db, context);
    //    }

    //    private static async Task CheckBlacklist(string token, AppDbContext db, TokenValidatedContext context)
    //    {
    //        var blacklisted = await db.BlacklistedTokens
    //            .AnyAsync(x => x.Token == token);

    //        if (blacklisted)
    //        {
    //            context.Fail("Token is blacklisted");
    //        }
    //    }
    //}
}
