using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;

namespace NuGet13794Example;

public static class Program
{
    private static JsonSerializerSettings _jsonSerializerSettings = new();
    public static JsonSerializerSettings JsonSerializerSettings => _jsonSerializerSettings;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string versionName = builder.Configuration["Swagger:VersionName"]!;
        string docTitle = builder.Configuration["Swagger:DocTitle"]!;

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

        // Add services to the container.

        builder.Services.AddControllers().AddNewtonsoftJson(options => {
            options.SerializerSettings.Formatting = Formatting.None;

            _jsonSerializerSettings = options.SerializerSettings;
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options => {
            var version = Assembly.GetExecutingAssembly().GetName().Version!;
            options.SwaggerDoc(versionName, new()
            {
                Version = $"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision}",
                Title = docTitle,
            });
            options.CustomSchemaIds(t => t.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? t.Name);
        });
        builder.Services.AddApplicationInsightsTelemetry();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        bool noSwagger = bool.TryParse(Environment.GetEnvironmentVariable("NO_SWAGGER"), out bool __noSwagger) && __noSwagger;
        if (app.Environment.IsDevelopment() && !noSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint($"/swagger/{versionName}/swagger.json", versionName));
        }

        app.MapGet("availability", () => Results.Ok("Ok"));

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
