using MealShareDotNet.Core.Data.DTOs;

namespace MealShareDotNet.Core.Services;

public interface IIngredientService
{
    // TODO: really good comments should probably go here
    Task<IEnumerable<IngredientListingDTO>> GetIngredienntListingsAsync(string query);
    Task<IngredientDTO> GetIngredientAsync(long id);
    Task<IngredientDTO> InsertIngredientAsync(IngredientDTO ingredient);
    Task<bool> DeleteIngredientAsync(long id);
    Task<IngredientDTO> UpdateIngredientAsync(IngredientDTO ingredient);
}
