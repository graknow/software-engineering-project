using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Repositories;

public interface IIngredientRepository
{
    Task<IEnumerable<Ingredient>> SearchIngredientsAsync(
            string? query = null,
            uint? pageSize = null,
            uint? pageOffset = null
            );

    Task<Ingredient?> GetIngredientByIdAsync(long id);
    Ingredient InsertIngredient(Ingredient ingredient);
    Task DeleteIngredientAsync(long id);
    Ingredient UpdateIngredient(Ingredient ingredient);
}
