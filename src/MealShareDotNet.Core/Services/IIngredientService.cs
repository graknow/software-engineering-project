using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Queries;

namespace MealShareDotNet.Core.Services;

public interface IIngredientService
{
    // TODO: really good comments should probably go here
    Task<IEnumerable<IngredientListingDTO>> GetIngredientListingsAsync(GetIngredientListingsQuery query);
    Task<IngredientDTO?> GetIngredientAsync(long id);
    Task<IngredientDTO> InsertIngredientAsync(IngredientDTO ingredient);
    Task DeleteIngredientAsync(long id);
    Task<IngredientDTO> UpdateIngredientAsync(IngredientDTO ingredient);
}
