using MealShareDotNet.Server.Auth;
using MealShareDotNet.Core.Repositories;
using MealShareDotNet.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var authConfigurationGenerator = (ApiKeyAuthSchemeOptions opts) =>
{
    var configs = builder.Configuration.GetSection("ApiKeys").Get<List<ApiKeyConfig>>();

    foreach (var config in configs ?? [])
    {
        opts.ApiKeys.Add(config.ToApiKey());
    }
};

builder.Services.AddAuthentication(ApiKeyAuthSchemeOptions.DefaultScheme)
    .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthSchemeHandler>(
            ApiKeyAuthSchemeOptions.DefaultScheme,
            authConfigurationGenerator
            );

var connString = builder.Configuration.GetConnectionString("Recipe");
if (connString is null)
{
    throw new Exception("Connection string not found in configuration settings.  Exiting...");
}

var fullConnString = ConnectionStringService.GenerateConnectionString(connString);

builder.Services.AddSingleton<IRecipeRepository>(new DbRecipeRepository(fullConnString));

var migrationService = new MigrationService(fullConnString, "Migrations");
migrationService.Migrate();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseAuthentication();
app.UseAuthorization();

app.UsePathBase("/api/");
app.UseRouting();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
