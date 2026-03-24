using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;
using MealShareDotNet.Core.Repositories;

namespace MealShareDotNet.Core.Tests.Unit.Mocks;

// TODO: Implement interface
public class RecipeRepositoryMock : IRecipeRepository
{
    private readonly List<Recipe> _recipes = [];
    private readonly List<Ingredient> _ingredients = [];
    private readonly List<RecipeIngredient> _ris = [];
    private readonly List<Tag> _tags = [];
    private readonly List<RecipeTag> _rts = [];

    public async Task DeleteIngredientAsync(long id)
    {
        var ingredient = _ingredients.SingleOrDefault(i => i.Id == id);

        if (ingredient is null)
        {
            throw new KeyNotFoundException();
        }

        _ingredients.Remove(ingredient);
        _ris.RemoveAll(ri => ri.IngredientId == id);
    }

    public async Task DeleteRecipeAsync(long id)
    {
        var recipe = _recipes.SingleOrDefault(r => r.Id == id);

        if (recipe is null)
        {
            throw new KeyNotFoundException();
        }

        _recipes.Remove(recipe);
        _ris.RemoveAll(ri => ri.RecipeId == id);
        _rts.RemoveAll(rt => rt.RecipeId == id);
    }

    public async Task DeleteTagAsync(long id)
    {
        var tag = _tags.SingleOrDefault(t => t.Id == id);

        if (tag is null)
        {
            throw new KeyNotFoundException();
        }

        _rts.RemoveAll(rt => rt.TagId == id);
    }

    public async Task<Ingredient?> GetIngredientByIdAsync(long id)
    {
        return _ingredients.SingleOrDefault(i => i.Id == id);
    }

    public async Task<Recipe?> GetRecipeByIdAsync(long id)
    {
        return _recipes.SingleOrDefault(r => r.Id == id);
    }

    public async Task<Tag?> GetTagByIdAsync(long id)
    {
        return _tags.SingleOrDefault(t => t.Id == id);
    }

    public async Task<Ingredient> InsertIngredientAsync(Ingredient ingredient)
    {
        var nextId = _ingredients.Select(i => i.Id).Max() + 1;

        ingredient.Id = nextId;
        _ingredients.Add(ingredient);

        return ingredient;
    }

    public async Task<Recipe> InsertRecipeAsync(Recipe recipe)
    {
        var nextId = _recipes.Select(r => r.Id).Max() + 1;

        recipe.Id = nextId;
        _recipes.Add(recipe);
    }

    public async Task<Tag> InsertTagAsync(Tag tag)
    {
        var nextId = _tags.Select(t => t.Id).Max() + 1;

        tag.Id = nextId;
        _tags.Add(tag);

        return tag;
    }

    public async Task<bool> RecipeExistsAsync(long id)
    {
        return _recipes.SingleOrDefault(r => r.Id == id) is not null;
    }

    public async Task<IEnumerable<Ingredient>> SearchIngredientsAsync(GetIngredientListingsQuery query)
    {
        var ingredients = _ingredients.Where(i => true);

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
        var tags = _tags.Where(i => true);

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
