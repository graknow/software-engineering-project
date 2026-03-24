using MealShareDotNet.Core.Repositories;
using MealShareDotNet.Core.Services;
using Microsoft.Extensions.DependencyInjection;
namespace MealShareDotNet.Client.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection services, string connection)
    {
        services.AddTransient<IRecipeRepository>(s => new SqliteRecipeRepository(connection));
        services.AddTransient<IRecipeService, RepositoryRecipeService>();
    }
}
