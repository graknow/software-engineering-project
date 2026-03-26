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

        _recipeRepository = new RecipeRepositoryMock();
    }
}
