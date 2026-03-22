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
public class SqliteRecipeRepositoryTests
{
    private const string _testConnectionString =
        "Data Source=SqliteRecipeRepoTests;Mode=Memory;Cache=Shared";

    private SqliteRecipeRepository _recipeRepository = default!;

    private IDbConnection _connection = default!;

    private static IEnumerable<Recipe> _recipes = [];
    private static IEnumerable<Ingredient> _ingredients = [];
    private static IEnumerable<Tag> _tags = [];
    private static IEnumerable<RecipeIngredient> _ris = [];
    private static IEnumerable<RecipeTag> _rts = [];

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

        foreach (var recipe in _recipes)
        {
            recipe.RecipeIngredients = _ris.Where(ri => ri.RecipeId == recipe.Id).ToList();
            recipe.RecipeTags = _rts.Where(rt => rt.RecipeId == recipe.Id).ToList();
        }

        foreach (var ri in _ris)
        {
            var relatedIngredient = _ingredients.Single(i => i.Id == ri.IngredientId);
            ri.Ingredient = relatedIngredient;
            ri.IngredientId = relatedIngredient.Id;
        }

        foreach (var rt in _rts)
        {
            var relatedTag = _tags.Single(t => t.Id == rt.TagId);
            rt.Tag = relatedTag;
            rt.TagId = relatedTag.Id;
        }

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
    public async Task SearchRecipes_NullParameters_ReturnAll()
    {
        var recipes = await _recipeRepository.SearchRecipesAsync(new());

        Assert.That(recipes.Count(), Is.EqualTo(_recipes.Count()));
    }

    // TODO: Add more recipe test data
    [TestCase(1, 1)]
    [TestCase(2, 1)]
    [Parallelizable]
    public async Task SearchRecipes_PageableParameters_ReturnOnePage(int pageSize, int pageNumber)
    {
        var pageOffset = pageSize * (pageNumber - 1);
        var query = new GetRecipeListingsQuery()
        {
            PageSize = pageSize,
            PageOffset = pageOffset
        };

        var expecteds = _recipes.Skip(pageOffset).Take(pageSize);
        var entities = await _recipeRepository.SearchRecipesAsync(query);

        Assert.That(entities.Count(), Is.EqualTo(pageSize));
    }

    [Test]
    [Parallelizable]
    public async Task GetRecipeById_InvalidId_ReturnNull()
    {
        var recipe = await _recipeRepository.GetRecipeByIdAsync(-1);

        Assert.That(recipe, Is.Null);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [Parallelizable]
    public async Task GetRecipeById_ValidId_ReturnFullRecipe(long id)
    {
        var recipe = await _recipeRepository.GetRecipeByIdAsync(id);
        var input = _recipes.First(r => r.Id == id);

        Assert.That(recipe, Is.Not.Null);
        Assert.That(recipe!.Name, Is.EqualTo(input.Name));
        Assert.That(recipe!.CookTime, Is.EqualTo(input.CookTime));
        Assert.That(recipe!.Price, Is.EqualTo(input.Price));
        Assert.That(recipe!.RecipeIngredients.Count(), Is.EqualTo(input.RecipeIngredients.Count()));
        Assert.That(recipe!.RecipeTags.Count(), Is.EqualTo(input.RecipeTags.Count()));

        foreach (var ri in recipe.RecipeIngredients)
        {
            var relatedRi = input.RecipeIngredients.First(rri => rri.IngredientId == ri.IngredientId);

            Assert.That(ri.RecipeId, Is.EqualTo(relatedRi.RecipeId));
            Assert.That(ri.Mass, Is.EqualTo(relatedRi.Mass));
            Assert.That(ri.Volume, Is.EqualTo(relatedRi.Volume));
            Assert.That(ri.Quantity, Is.EqualTo(relatedRi.Quantity));
        }

        foreach (var rt in recipe.RecipeTags)
        {
            var relatedRt = input.RecipeTags.First(rrt => rrt.TagId == rt.TagId);

            Assert.That(rt.RecipeId, Is.EqualTo(relatedRt.RecipeId));
        }
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

    [TestCase(1)]
    public async Task DeleteRecipe_ValidId_RemovedFromDatabase(long id)
    {
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

    [TestCase(-1)]
    [TestCase(25565)]
    public void DeleteRecipe_InvalidId_KeyNotFoundException(long id)
    {
        try
        {
            _recipeRepository.BeginTransaction();
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _recipeRepository.DeleteRecipeAsync(id));
        }
        finally
        {
            _recipeRepository.Rollback();
        }
    }

    [Test]
    public async Task UpdateRecipe_ValidRecipe_UpdatedInDatabase()
    {
        var recipe = _recipes.First();

        var updated = new Recipe()
        {
            Id = recipe.Id,
            Name = recipe.Name + "Updated",
            CookTime = recipe.CookTime + 1,
            Price = recipe.Price + 1,
            ServingQuantity = recipe.ServingQuantity + 1,
            Instructions = recipe.Instructions,
            RecipeIngredients = _ris
                .Select(ri =>
                {
                    ri.Recipe = recipe;
                    ri.RecipeId = recipe.Id;
                    return ri;
                }).Take(4).ToList(),
            RecipeTags = _rts
                .Select(rt =>
                {
                    rt.Recipe = recipe;
                    rt.RecipeId = recipe.Id;
                    return rt;
                }).Take(2).ToList(),
        };

        Recipe result;

        try
        {
            _recipeRepository.BeginTransaction();

            result = await _recipeRepository.UpdateRecipeAsync(updated);
        }
        finally
        {
            _recipeRepository.Rollback();
        }


        Assert.That(result.Name, Is.EqualTo(updated.Name));
        Assert.That(result.CookTime, Is.EqualTo(updated.CookTime));
        Assert.That(result.Price, Is.EqualTo(updated.Price));
        Assert.That(result.ServingQuantity, Is.EqualTo(updated.ServingQuantity));
        Assert.That(result.Instructions, Is.EqualTo(updated.Instructions));

        Assert.That(result.RecipeIngredients.Count(), Is.EqualTo(updated.RecipeIngredients.Count()));
        Assert.That(result.RecipeTags.Count(), Is.EqualTo(updated.RecipeTags.Count()));
    }

    [Test]
    [Parallelizable]
    public async Task SearchIngredients_NullParameters_ReturnAll()
    {
        var ingredients = await _recipeRepository.SearchIngredientsAsync(new());

        Assert.That(ingredients.Count(), Is.EqualTo(_ingredients.Count()));
    }

    [TestCase(1, 1)]
    [TestCase(2, 2)]
    [Parallelizable]
    public async Task SearchIngredients_PageableParameters_ReturnOnePage(int pageSize, int pageNumber)
    {
        var pageOffset = pageSize * (pageNumber - 1);
        var query = new GetIngredientListingsQuery()
        {
            PageSize = pageSize,
            PageOffset = pageOffset
        };

        var expecteds = _ingredients.Skip(pageOffset).Take(pageSize);
        var entities = await _recipeRepository.SearchIngredientsAsync(query);

        Assert.That(entities.Count(), Is.EqualTo(pageSize));
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

    [TestCase(1)]
    [TestCase(2)]
    [Parallelizable]
    public async Task RecipeExists_ValidId_ReturnTrue(long id)
    {
        var exists = await _recipeRepository.RecipeExistsAsync(id);

        Assert.That(exists, Is.True);
    }

    [TestCase(-1)]
    [TestCase(25565)]
    public async Task RecipeExists_InvalidId_ReturnFalse(long id)
    {
        var exists = await _recipeRepository.RecipeExistsAsync(id);

        Assert.That(exists, Is.False);
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

    [TestCase(1)]
    public async Task DeleteIngredient_ValidId_RemovedFromDatabase(long id)
    {
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

    [TestCase(-1)]
    [TestCase(25565)]
    public void DeleteIngredient_InvalidId_KeyNotFoundException(long id)
    {
        try
        {
            _recipeRepository.BeginTransaction();
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _recipeRepository.DeleteIngredientAsync(id));
        }
        finally
        {
            _recipeRepository.Rollback();
        }
    }

    [Test]
    public async Task UpdateIngredient_ValidIngredient_UpdatedInDatabase()
    {
        var ingredient = _ingredients.First();
        var updated = new Ingredient()
        {
            Id = ingredient.Id,
            Name = ingredient.Name + "Updated",
        };

        Ingredient entity;

        try
        {
            _recipeRepository.BeginTransaction();
            entity = await _recipeRepository.UpdateIngredientAsync(updated);
        }
        finally
        {
            _recipeRepository.Rollback();
        }

        Assert.That(entity.Name, Is.EqualTo(updated.Name));
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

    [TestCase(1)]
    public async Task DeleteTag_ValidId_RemovedFromDatabase(long id)
    {
        bool exists;

        try
        {
            _recipeRepository.BeginTransaction();

            await _recipeRepository.DeleteTagAsync(id);
            exists = (await _recipeRepository.GetTagByIdAsync(id)) is not null;
        }
        finally
        {
            _recipeRepository.Rollback();
        }

        Assert.That(exists, Is.False);
    }

    [TestCase(-1)]
    [TestCase(25565)]
    public void DeleteTag_InvalidId_KeyNotFoundException(long id)
    {
        try
        {
            _recipeRepository.BeginTransaction();
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _recipeRepository.DeleteTagAsync(id));
        }
        finally
        {
            _recipeRepository.Rollback();
        }
    }

    [Test]
    public async Task UpdateTag_ValidTag_UpdatedInDatabase()
    {
        var tag = _tags.First();
        var updated = new Tag()
        {
            Id = tag.Id,
            Name = tag.Name + "Updated",
            Description = tag.Description ?? String.Empty + "Updated"
        };

        Tag entity;

        try
        {
            _recipeRepository.BeginTransaction();
            entity = await _recipeRepository.UpdateTagAsync(updated);
        }
        finally
        {
            _recipeRepository.Rollback();
        }

        Assert.That(entity.Name, Is.EqualTo(updated.Name));
        Assert.That(entity.Description, Is.EqualTo(updated.Description));
    }
}
