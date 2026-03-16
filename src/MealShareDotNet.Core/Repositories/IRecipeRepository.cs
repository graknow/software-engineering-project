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
}
