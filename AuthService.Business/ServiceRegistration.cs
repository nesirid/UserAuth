
using AuthService.Business.Services.AuthService.Interface;
using AuthService.Business.Services.AuthService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.HelperServices.Current;
using AuthService.Business.Services.TokenHandler;

namespace AuthService.Business
{
    public static class ServiceRegistration
    {
        public static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddHttpContextAccessor();

            services.AddScoped<ICurrentUser, CurrentUser>();

            services.AddScoped<IAuthService, AuthManager>();

            services.AddScoped<ITokenHandler, TokenHandler>();
        }

        
    }
}
