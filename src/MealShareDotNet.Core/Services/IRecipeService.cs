using MealShareDotNet.Core.Data.DTOs;

namespace MealShareDotNet.Core.Services;

public interface IRecipeService
{
    Task<IEnumerable<RecipeListingDTO>> GetRecipeListingsAsync(uint? pageSize, uint? pageOffset);
    Task<RecipeDTO> GetRecipeAsync(long id);
    Task<bool> DeleteRecipeAsync(long id);
}
