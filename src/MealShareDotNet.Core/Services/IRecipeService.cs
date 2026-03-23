using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Queries;

namespace MealShareDotNet.Core.Services;

public interface IRecipeService
{
    Task<IEnumerable<RecipeListingDTO>> GetRecipeListingsAsync(GetRecipeListingsQuery query);
    Task<RecipeDTO?> GetRecipeAsync(long id);
    Task<RecipeDTO> InsertRecipeAsync(RecipeDTO recipe);
    Task DeleteRecipeAsync(long id);
    Task<RecipeDTO> UpdateRecipeAsync(RecipeDTO recipe);
    // TODO: Patch?
}
