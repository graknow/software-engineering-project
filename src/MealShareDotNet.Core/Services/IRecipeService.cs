using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Queries;

namespace MealShareDotNet.Core.Services;

public interface IRecipeService
{
    string Name { get; set; }
    Task<IEnumerable<RecipeListingDTO>> GetRecipeListingsAsync(GetRecipeListingsQuery query);
    Task<RecipeDTO?> GetRecipeAsync(long id);
    Task<RecipeDTO?> GetRandomDailyRecipeAsync();
    Task<RecipeDTO> InsertRecipeAsync(RecipeDTO recipe);
    Task DeleteRecipeAsync(long id);
    Task<RecipeDTO> UpdateRecipeAsync(RecipeDTO recipe);
    // TODO: Patch?
}
