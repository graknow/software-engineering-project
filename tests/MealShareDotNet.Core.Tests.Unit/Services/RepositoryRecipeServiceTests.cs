using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using YamlDotNet.Serialization;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;
using MealShareDotNet.Core.Tests.Unit.Mocks;
using MealShareDotNet.Core.Repositories;
using MealShareDotNet.Core.Services;

namespace MealShareDotNet.Core.Tests.Unit.Services;

[TestFixture]
public class RecipeRepositoryServiceTests
{
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
        _recipeRepository = new MockRecipeRepository();
        _recipeService = new RepositoryRecipeService(_recipeRepository);
    }

    [TestCase(0)]
    [TestCase(1)]
    public async Task GetRecipe_ValidId_FullRecipe(long id)
    {
        var recipe = await _recipeRepository.GetRecipeByIdAsync(id);

        var entity = await _recipeService.GetRecipeAsync(id);

        Assert.That(entity?.Name, Is.EqualTo(recipe?.Name));
    }

    [Test]
    public async Task InsertRecipe_ValidRecipe_AddedToService()
    {
        var entity = new Recipe()
        {
            Name = "TestInsertion",
            Instructions = "TestInstructions",
        };
        // TODO: tests are a later problem i guess
    }
}
