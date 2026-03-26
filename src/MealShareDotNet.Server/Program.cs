using System.Threading.RateLimiting;
using MealShareDotNet.Server.Auth;
using MealShareDotNet.Core.Repositories;
using MealShareDotNet.Core.Services;
using MealShareDotNet.Core.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// move options to appsettings
builder.Services.AddRateLimiter(opts =>
{
    opts.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(http =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 30,
                    QueueLimit = 0,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                }));
});

var authConfigurationGenerator = (ApiKeyAuthSchemeOptions opts) =>
{
    var configs = builder.Configuration.GetSection("ApiKeys").Get<List<ApiKeyConfig>>();

    foreach (var config in configs ?? [])
    {
        if (ApiKey.TryParse(config, out var key))
        {
            opts.ApiKeys.Add(key);
        }
        else
        {
            Console.WriteLine($"WARNING - Failed to parse configuration key with name \"{config.Name}\".  Skipping...");
        }
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

var fullConnString = ConnectionStringUtil.GenerateConnectionString(connString);

builder.Services.AddTransient<IRecipeRepository>(s => new SqliteRecipeRepository(fullConnString));
builder.Services.AddScoped<IRecipeService, RepositoryRecipeService>();
builder.Services.AddScoped<IIngredientService, RepositoryIngredientService>();
builder.Services.AddScoped<ITagService, RepositoryTagService>();

var migrationService = new MigrationService(fullConnString, "Migrations");
migrationService.Migrate();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UsePathBase("/api/");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
