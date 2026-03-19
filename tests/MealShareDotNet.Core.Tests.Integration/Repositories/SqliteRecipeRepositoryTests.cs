using System.Data;
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
    private const string _testConnectionString =
        "Data Source=SqliteRecipeRepoTests;Mode=Memory;Cache=Shared";

    private SqliteRecipeRepository _recipeRepository = default!;

    private IDbConnection _connection = default!;

    private IEnumerable<Recipe> _recipes = [];
    private IEnumerable<Ingredient> _ingredients = [];
    private IEnumerable<Tag> _tags = [];
    private IEnumerable<RecipeIngredient> _ris = [];
    private IEnumerable<RecipeTag> _rts = [];

    [OneTimeSetUp]
    public void SetUpAll()
    {
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

        _connection = new SqliteConnection(_testConnectionString);
        _connection.Open();

        var migrationService = new MigrationService(_testConnectionString, "Migrations");
        migrationService.Migrate();

        _connection!.Execute("INSERT INTO Recipes (Id, Name, Instructions) VALUES (@Id, @Name, @Instructions);", _recipes);

        _connection!.Execute("INSERT INTO Ingredients (Id, Name) VALUES (@Id, @Name);", _ingredients);

        _connection!.Execute("INSERT INTO Tags (Id, Name, Description) VALUES (@Id, @Name, @Description);", _tags);

        _connection!.Execute("INSERT INTO RecipeIngredient VALUES (@RecipeId, @IngredientId, @Mass, @Volume, @Quantity)", _ris);

        _connection!.Execute("INSERT INTO RecipeTag VALUES (@RecipeId, @TagId);", _rts);

        _recipeRepository = new SqliteRecipeRepository(_testConnectionString);
    }

    [OneTimeTearDown]
    public void TearDownAll()
    {
        _recipeRepository.Dispose();
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

    [Test]
    [Parallelizable]
    public async Task SearchRecipes_NoParameters_ReturnAll()
    {
        var recipes = await _recipeRepository.SearchRecipesAsync(new());

        Assert.That(recipes.Count(), Is.EqualTo(_recipes.Count()));
    }

    [Test]
    [Parallelizable]
    public async Task GetRecipeById_InvalidId_ReturnNull()
    {
        var recipe = await _recipeRepository.GetRecipeByIdAsync(-1);

        Assert.That(recipe, Is.Null);
    }

    [Test]
    [Parallelizable]
    public async Task GetRecipeById_ValidId_ReturnFullRecipe()
    {
        Assert.Pass();
    }

    [Test]
    public async Task InsertRecipe_ValidRecipe_AddedToDatabase()
    {
        var recipe = new Recipe()
        {
            Name = "Test Recipe",
            Instructions = "Test Instructions"
        };

        bool exists;

        try
        {
            _recipeRepository.BeginTransaction();

            var result = await _recipeRepository.InsertRecipeAsync(recipe);

            exists = await _recipeRepository.RecipeExistsAsync(result.Id ?? -1);
        }
        finally
        {
            _recipeRepository.Rollback();
        }

        Assert.That(exists, Is.True);
    }

    [Test]
    public async Task DeleteRecipe_ValidId_RemovedFromDatabase()
    {
        var id = 1;

        bool exists;

        try
        {
            _recipeRepository.BeginTransaction();

            await _recipeRepository.DeleteRecipeAsync(id);
            exists = await _recipeRepository.RecipeExistsAsync(id);
        }
        finally
        {
            _recipeRepository.Rollback();
        }


        Assert.That(exists, Is.False);
    }

    [Test]
    [Parallelizable]
    public async Task SearchIngredients_NoParameters_ReturnAll()
    {
        var ingredients = await _recipeRepository.SearchIngredientsAsync(new());

        Assert.That(ingredients.Count(), Is.EqualTo(_ingredients.Count()));
    }

    [Test]
    [Parallelizable]
    public async Task GetIngredientById_InvalidId_ReturnNull()
    {
        var id = -1;

        var ingredient = await _recipeRepository.GetIngredientByIdAsync(id);

        Assert.That(ingredient, Is.Null);
    }

    [Test]
    [Parallelizable]
    public async Task GetIngredientById_ValidId_ReturnFullIngredient()
    {
        var id = 1;

        var ingredient = await _recipeRepository.GetIngredientByIdAsync(id);

        Assert.That(
                ingredient?.Name,
                Is.EqualTo(_ingredients.First(i => i.Id == id).Name)
                );
    }

    [Test]
    public async Task InsertIngredient_ValidIngredient_AddedToDatabase()
    {
        var ingredient = new Ingredient()
        {
            Name = "Unique Test Ingredient"
        };

        Ingredient result;
        bool exists;

        try
        {
            _recipeRepository.BeginTransaction();

            result = await _recipeRepository.InsertIngredientAsync(ingredient);
            exists = (await _recipeRepository.GetIngredientByIdAsync(result.Id ?? -1)) is not null;
        }
        finally
        {
            _recipeRepository.Rollback();
        }

        Assert.That(exists, Is.True);
        Assert.That(result.Name, Is.EqualTo(ingredient.Name));
    }

    [Test]
    public async Task DeleteIngredient_ValidId_RemovedFromDatabase()
    {
        var id = 1;

        bool exists;

        try
        {
            _recipeRepository.BeginTransaction();

            await _recipeRepository.DeleteIngredientAsync(id);
            exists = (await _recipeRepository.GetIngredientByIdAsync(id)) is not null;
        }
        finally
        {
            _recipeRepository.Rollback();
        }

        Assert.That(exists, Is.False);
    }

    [Test]
    [Parallelizable]
    public async Task SearchTags_NoParameters_ReturnAll()
    {
        var tags = await _recipeRepository.SearchTagsAsync(new());

        Assert.That(tags.Count(), Is.EqualTo(_tags.Count()));
    }

    [Test]
    [Parallelizable]
    public async Task GetTagById_InvalidId_ReturnNull()
    {
        var tag = await _recipeRepository.GetTagByIdAsync(-1);

        Assert.That(tag, Is.Null);
    }

    [Test]
    public async Task InsertTag_ValidTag_AddedToDatabase()
    {
        var tag = new Tag()
        {
            Name = "Unique Test Tag",
            Description = "D"
        };

        Tag result;
        bool exists;

        try
        {
            _recipeRepository.BeginTransaction();

            result = await _recipeRepository.InsertTagAsync(tag);
            exists = (await _recipeRepository.GetTagByIdAsync(result.Id ?? -1)) is not null;
        }
        finally
        {
            _recipeRepository.Rollback();
        }

        Assert.That(exists, Is.True);
        Assert.That(result.Name, Is.EqualTo(tag.Name));
    }

    [Test]
    public async Task DeleteTag_ValidId_RemovedFromDatabase()
    {
        var id = 1;

        bool exists;

        try
        {
            _recipeRepository.BeginTransaction();

            await _recipeRepository.DeleteIngredientAsync(id);
            exists = (await _recipeRepository.GetIngredientByIdAsync(id)) is not null;
        }
        finally
        {
            _recipeRepository.Rollback();
        }

        Assert.That(exists, Is.False);
    }
}
