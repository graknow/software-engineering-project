using MealShareDotNet.Core.Services;
using MealShareDotNet.Core.Utils;
using System;
using Avalonia;

namespace MealShareDotNet.Client.Desktop;

sealed class Program
{
    //builder.Services.AddTransient<IRecipeRepository>(s => new SqliteRecipeRepository(fullConnString));
   // builder.Services.AddTransient<IRecipeService, RepositoryRecipeService>();
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var fullConnString = ConnectionStringUtil.GenerateConnectionString("DataSource=recipe.db;Cache=Shared");
        var migrationService = new MigrationService(fullConnString, "Migrations");
        migrationService.Migrate();



        BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);


    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
