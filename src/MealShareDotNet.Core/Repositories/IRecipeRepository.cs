using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Repositories;

public interface IRecipeRepository
{
    Task<IEnumerable<Recipe>> SearchRecipesAsync(
            string? query = null,
            uint? pageSize = null,
            uint? pageOffset = null
            );

    Task<Recipe?> GetRecipeByIdAsync(long id);
    Task<Recipe> InsertRecipeAsync(Recipe recipe);
    Task DeleteRecipeAsync(long id);
    Recipe UpdateRecipe(Recipe recipe);


    Task<IEnumerable<Ingredient>> SearchIngredientsAsync(
            string? query = null,
            uint? pageSize = null,
            uint? pageOffset = null
            );

    Task<Ingredient?> GetIngredientByIdAsync(long id);
    Ingredient InsertIngredient(Ingredient ingredient);
    Task DeleteIngredientAsync(long id);
    Ingredient UpdateIngredient(Ingredient ingredient);


    Task<IEnumerable<Tag>> SearchTagsAsync(
            string? query = null,
            uint? pageSize = null,
            uint? pageOffset = null
            );

    Task<Tag?> GetTagByIdAsync(long id);
    Tag InsertTag(Tag tag);
    Task DeleteTagAsync(long id);
    Tag UpdateTag(Tag tag);
}
