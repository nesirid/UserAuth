using AuthService.Business;
using AuthService.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

builder.Services.AddAuthServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI v1");

        c.ConfigObject.AdditionalItems["persistAuthorization"] = "true";

        c.OAuthClientId("swagger-ui");
        c.OAuthAppName("Swagger UI");
        c.OAuthUsePkce();
    });
}



app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseCustomExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
