
using AuthService.Business.Services.AuthService.Interface;
using AuthService.Business.Services.AuthService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.HelperServices.Current;
using AuthService.Business.Services.TokenHandler;
using AuthService.Business.Services.TokenHandler.Interface;
using SharedLibrary.HelperServices.Current.Interface;
using AuthService.Business.Dtos;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace AuthService.Business
{
    public static class ServiceRegistration
    {
        public static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            // HttpContextAccessor əlavə edir ki, bu da cari istifadəçi məlumatlarına daxil olmağa imkan verir
            services.AddHttpContextAccessor();

            // Service Registration üçün istifadə olunur
            services.AddScoped<ICurrentUser, CurrentUser>();

            services.AddScoped<IAuthService, AuthManager>();

            services.AddScoped<ITokenHandler, TokenHandler>();



            services.AddFluentValidationAutoValidation();// FluentValidation üçün avtomatik doğrulama əlavə edir



            // Dto doğrulayıcılarını qeydiyyatdan keçirir
            services.AddValidatorsFromAssemblyContaining<PasswordResetDtoValidator>();

            services.AddValidatorsFromAssemblyContaining<PasswordChangeDtoValidator>();

            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();

            services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();



        }

    }
}
