using Microsoft.Data.Sqlite;
using Dapper;
using YamlDotNet.Serialization;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Repositories;
using MealShareDotNet.Core.Services;

namespace MealShareDotNet.Core.Tests.Integration.Repositories;

[TestFixture]
public class SqliteRecipeRepositoryTests
{
    private const string _testConnectionString = "Data Source=SqliteRecipeRepoTest;Mode=Memory;Cache=Shared";

    private SqliteConnection? _connection;
    private SqliteRecipeRepository _recipeRepository = default!;

    private IEnumerable<Recipe> _recipes = [];
    private IEnumerable<Ingredient> _ingredients = [];
    private IEnumerable<Tag> _tags = [];
    private IEnumerable<RecipeIngredient> _ris = [];
    private IEnumerable<RecipeTag> _rts = [];

    [OneTimeSetUp]
    public void SetupAll()
    {
        _connection = new SqliteConnection(_testConnectionString);

        _recipeRepository = new SqliteRecipeRepository(_testConnectionString);

        var deserializer = new Deserializer();

        _recipes = deserializer.Deserialize<IEnumerable<Recipe>>(
                new StreamReader("test-data/tables/Recipes.yaml")
                );

        _ingredients = deserializer.Deserialize<IEnumerable<Ingredient>>(
                new StreamReader("test-data/tables/Ingredients.yaml")
                );

        _tags = deserializer.Deserialize<IEnumerable<Tag>>(
                new StreamReader("test-data/tables/Tags.yaml")
                );

        _ris = deserializer.Deserialize<IEnumerable<RecipeIngredient>>(
                new StreamReader("test-data/tables/RecipeIngredient.yaml")
                );

        _rts = deserializer.Deserialize<IEnumerable<RecipeTag>>(
                new StreamReader("test-data/tables/RecipeTag.yaml")
                );
    }

    [OneTimeTearDown]
    public void TeardownAll()
    {
        _connection?.Dispose();
        _connection = null;
        _recipeRepository?.Dispose();
    }

    [SetUp]
    public void Setup()
    {
        _connection!.Open();

        var migrationService = new MigrationService(_testConnectionString, "Migrations");

        migrationService.Migrate();

        _connection!.Execute("INSERT INTO Recipes (ID, Name, Instructions) VALUES (@ID, @Name, @Instructions);", _recipes);

        _connection!.Execute("INSERT INTO Ingredients (ID, Name) VALUES (@ID, @Name);", _ingredients);

        _connection!.Execute("INSERT INTO Tags (ID, Name, Description) VALUES (@ID, @Name, @Description);", _tags);

        _connection!.Execute("INSERT INTO RecipeIngredient VALUES (@RecipeID, @IngredientID, @Mass, @Volume, @Quantity)", _ris);

        _connection!.Execute("INSERT INTO RecipeTag VALUES (@RecipeID, @TagID);", _rts);
    }

    [TearDown]
    public void TearDown()
    {
        _connection?.Close();
    }

    [Test]
    public async Task SearchRecipes_NoParameters_ReturnAll()
    {
        var recipes = await _recipeRepository.SearchRecipesAsync(new());

        Assert.That(recipes.Count(), Is.EqualTo(_recipes.Count()));
    }
}
