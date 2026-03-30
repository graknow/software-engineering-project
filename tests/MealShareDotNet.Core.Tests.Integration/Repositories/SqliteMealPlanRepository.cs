using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using YamlDotNet.Serialization;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;
using MealShareDotNet.Core.Repositories;
using MealShareDotNet.Core.Services;

namespace MealShareDotNet.Core.Tests.Integration.Repositories;

[TestFixture]
public class SqliteMealPlanRepositoryTests
{
    private const string _testConnectionString =
        "Data Source=SqliteRecipeRepoTests;Mode=Memory;Cache=Shared";

    private SqliteMealPlanRepository _mealPlanRepository = default!;

    private IDbConnection _connection = default!;

    private static IEnumerable<Recipe> _recipes = [];

    [OneTimeSetUp]
    public void SetUpAll()
    {
        var deserializer = new Deserializer();

        _recipes = deserializer.Deserialize<IEnumerable<Recipe>>(
                new StreamReader("test-data/tables/Recipes.yaml")
                );

        _connection = new SqliteConnection(_testConnectionString);
        _connection.Open();

        var migrationService = new MigrationService(_testConnectionString, "Migrations");
        migrationService.Migrate();

        _connection!.Execute("INSERT INTO Recipes (Id, Name, Instructions) VALUES (@Id, @Name, @Instructions);", _recipes);

        _mealPlanRepository = new SqliteMealPlanRepository(_testConnectionString);
    }

    [OneTimeTearDown]
    public void TearDownAll()
    {
        _connection.Dispose();
    }

    [SetUp]
    public void SetUp()
    {
    }

    [TearDown]
    public void TearDown()
    {
    }
}
