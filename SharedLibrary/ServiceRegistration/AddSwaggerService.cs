using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using SharedLibrary.Filters;

namespace SharedLibrary.ServiceRegistration
{
    public static class AddSwaggerService
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(opt =>
            {
                opt.OperationFilter<AddLanguageHeaderParameter>();
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });

                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Flows = new OpenApiOAuthFlows() 
                });

                opt.OperationFilter<SecurityRequirementsOperationFilter>();

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            });
        }
    }
}