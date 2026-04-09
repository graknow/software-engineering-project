using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;

namespace MealShareDotNet.Core.Repositories;

public interface IRecipeRepository
{
    Task<IEnumerable<Recipe>> SearchRecipesAsync(GetRecipeListingsQuery query);
    Task<bool> RecipeExistsAsync(long id);
    Task<long> GetRecipeCount();
    Task<Recipe?> GetRecipeByIdAsync(long id);
    Task<Recipe> InsertRecipeAsync(Recipe recipe);
    Task DeleteRecipeAsync(long id);
    Task<Recipe> UpdateRecipeAsync(Recipe recipe);


    Task<IEnumerable<Ingredient>> SearchIngredientsAsync(GetIngredientListingsQuery query);
    Task<Ingredient?> GetIngredientByIdAsync(long id);
    Task<Ingredient> InsertIngredientAsync(Ingredient ingredient);
    Task DeleteIngredientAsync(long id);
    Task<Ingredient> UpdateIngredientAsync(Ingredient ingredient);


    Task<IEnumerable<Tag>> SearchTagsAsync(GetTagListingsQuery query);
    Task<Tag?> GetTagByIdAsync(long id);
    Task<Tag> InsertTagAsync(Tag tag);
    Task DeleteTagAsync(long id);
    Task<Tag> UpdateTagAsync(Tag tag);
}
