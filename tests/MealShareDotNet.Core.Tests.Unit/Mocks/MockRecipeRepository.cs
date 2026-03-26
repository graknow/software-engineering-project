using YamlDotNet.Serialization;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;
using MealShareDotNet.Core.Repositories;

namespace MealShareDotNet.Core.Tests.Unit.Mocks;

// TODO: Implement interface
public class MockRecipeRepository : IRecipeRepository
{
    public readonly List<Recipe> Recipes = [];
    public readonly List<Ingredient> Ingredients = [];
    public readonly List<RecipeIngredient> Ris = [];
    public readonly List<Tag> Tags = [];
    public readonly List<RecipeTag> Rts = [];

    public MockRecipeRepository()
    {
        var deserializer = new Deserializer();

        Recipes = deserializer.Deserialize<IEnumerable<Recipe>>(
                new StreamReader("test-data/tables/Recipes.yaml")
                ).ToList();

        Ingredients = deserializer.Deserialize<IEnumerable<Ingredient>>(
                new StreamReader("test-data/tables/Ingredients.yaml")
                ).ToList();

        Tags = deserializer.Deserialize<IEnumerable<Tag>>(
                new StreamReader("test-data/tables/Tags.yaml")
                ).ToList();

        Ris = deserializer.Deserialize<IEnumerable<RecipeIngredient>>(
                new StreamReader("test-data/tables/RecipeIngredient.yaml")
                ).ToList();

        Rts = deserializer.Deserialize<IEnumerable<RecipeTag>>(
                new StreamReader("test-data/tables/RecipeTag.yaml")
                ).ToList();

        foreach (var recipe in Recipes)
        {
            recipe.RecipeIngredients = Ris.Where(ri => ri.RecipeId == recipe.Id).ToList();
            recipe.RecipeTags = Rts.Where(rt => rt.RecipeId == recipe.Id).ToList();
        }

        foreach (var ri in Ris)
        {
            var relatedIngredient = Ingredients.Single(i => i.Id == ri.IngredientId);
            ri.Ingredient = relatedIngredient;
            ri.IngredientId = relatedIngredient.Id;
        }

        foreach (var rt in Rts)
        {
            var relatedTag = Tags.Single(t => t.Id == rt.TagId);
            rt.Tag = relatedTag;
            rt.TagId = relatedTag.Id;
        }
    }

    public async Task DeleteIngredientAsync(long id)
    {
        var ingredient = Ingredients.SingleOrDefault(i => i.Id == id);

        if (ingredient is null)
        {
            throw new KeyNotFoundException();
        }

        Ingredients.Remove(ingredient);
        Ris.RemoveAll(ri => ri.IngredientId == id);
    }

    public async Task DeleteRecipeAsync(long id)
    {
        var recipe = Recipes.SingleOrDefault(r => r.Id == id);

        if (recipe is null)
        {
            throw new KeyNotFoundException();
        }

        Recipes.Remove(recipe);
        Ris.RemoveAll(ri => ri.RecipeId == id);
        Rts.RemoveAll(rt => rt.RecipeId == id);
    }

    public async Task DeleteTagAsync(long id)
    {
        var tag = Tags.SingleOrDefault(t => t.Id == id);

        if (tag is null)
        {
            throw new KeyNotFoundException();
        }

        Rts.RemoveAll(rt => rt.TagId == id);
    }

    public async Task<Ingredient?> GetIngredientByIdAsync(long id)
    {
        return (Ingredient?)Ingredients.SingleOrDefault(i => i.Id == id)?.Clone();
    }

    public async Task<Recipe?> GetRecipeByIdAsync(long id)
    {
        return (Recipe?)Recipes.SingleOrDefault(r => r.Id == id)?.Clone();
    }

    public async Task<Tag?> GetTagByIdAsync(long id)
    {
        return (Tag?)Tags.SingleOrDefault(t => t.Id == id)?.Clone();
    }

    public async Task<Ingredient> InsertIngredientAsync(Ingredient ingredient)
    {
        var nextId = Ingredients.Select(i => i.Id).Max() + 1;
        var clone = (Ingredient)ingredient.Clone();

        clone.Id = nextId;
        Ingredients.Add(clone);

        return (Ingredient)clone.Clone();
    }

    public async Task<Recipe> InsertRecipeAsync(Recipe recipe)
    {
        var nextId = Recipes.Select(r => r.Id).Max() + 1;
        var clone = (Recipe)recipe.Clone();

        clone.Id = nextId;
        Recipes.Add(clone);

        return (Recipe)clone.Clone();
    }

    public async Task<Tag> InsertTagAsync(Tag tag)
    {
        var nextId = Tags.Select(t => t.Id).Max() + 1;
        var clone = (Tag)tag.Clone();

        clone.Id = nextId;
        Tags.Add(clone);

        return (Tag)clone.Clone();
    }

    public async Task<bool> RecipeExistsAsync(long id)
    {
        return Recipes.SingleOrDefault(r => r.Id == id) is not null;
    }

    public async Task<IEnumerable<Ingredient>> SearchIngredientsAsync(GetIngredientListingsQuery query)
    {
        var ingredients = Ingredients.Where(i => true);

        if (query.PageOffset is not null && query.PageSize is not null)
        {
            ingredients = ingredients
                .Skip(query.PageOffset ?? -1)
                .Take(query.PageSize ?? -1);
        }

        return ingredients;
    }

    public async Task<IEnumerable<Recipe>> SearchRecipesAsync(GetRecipeListingsQuery query)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Tag>> SearchTagsAsync(GetTagListingsQuery query)
    {
        var tags = Tags.Where(i => true);

        if (query.PageOffset is not null && query.PageSize is not null)
        {
            tags = tags
                .Skip(query.PageOffset ?? -1)
                .Take(query.PageSize ?? -1);
        }

        return tags;
    }

    public Task<Ingredient> UpdateIngredientAsync(Ingredient ingredient)
    {
        throw new NotImplementedException();
    }

    public Task<Recipe> UpdateRecipeAsync(Recipe recipe)
    {
        throw new NotImplementedException();
    }

    public Task<Tag> UpdateTagAsync(Tag tag)
    {
        throw new NotImplementedException();
    }
}
