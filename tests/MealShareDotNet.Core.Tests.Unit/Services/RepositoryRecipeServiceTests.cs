using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using YamlDotNet.Serialization;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;
using MealShareDotNet.Core.Tests.Unit.Mocks;
using MealShareDotNet.Core.Repositories;
using MealShareDotNet.Core.Services;

namespace MealShareDotNet.Core.Tests.Unit.Services;

[TestFixture]
public class RecipeRepositoryServiceTests
{
    private const string _testConnectionString =
        "Data Source=SqliteRecipeRepoTests;Mode=Memory;Cache=Shared";

    private IRecipeRepository _recipeRepository = default!;
    private IRecipeService _recipeService = default!;

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

        _recipeRepository = new MockRecipeRepository();
        _recipeService = new RepositoryRecipeService(_recipeRepository);
    }

    [OneTimeTearDown]
    public void TearDownAll()
    {
    }

    [TestCase(0)]
    [TestCase(1)]
    public async Task GetRecipe_ValidId_FullRecipe(long id)
    {
        var entity = await _recipeRepository.GetRecipeByIdAsync(id);

        var recipe = await _recipeService.GetRecipeAsync(id);

        Assert.That(recipe?.Name, Is.EqualTo(entity?.Name));
    }

    [Test]
    public async Task GetRandomDailyRecipe_NoParameters_FullRecipe()
    {
        var recipe = await _recipeService.GetRandomDailyRecipeAsync();
        
        // Should get the same recipe on the same day
        Assert.That(recipe, Is.Not.Null);
        Assert.That((await _recipeService.GetRandomDailyRecipeAsync())?.Name, Is.EqualTo(recipe?.Name));
    }

    [Test]
    public async Task InsertRecipe_ValidRecipe_AddedToService()
    {
        var recipe = new RecipeDTO()
        {
            Name = "TestInsertion",
            Instructions = "TestInstructions",
            Tags = [
                new() { Id = null, Name = "TestTag1"},
                new() { Id = 1 }
            ]
        };

        var id = (await _recipeService.InsertRecipeAsync(recipe)).Id;
        var inserted = await _recipeService.GetRecipeAsync(id ?? -1);

        Assert.That(inserted?.Name, Is.EqualTo(recipe?.Name));
        Assert.That(inserted?.Tags.Count, Is.EqualTo(recipe?.Tags.Count));
    }
}
