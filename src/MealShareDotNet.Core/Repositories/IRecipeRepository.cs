using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Requests;

namespace MealShareDotNet.Core.Repositories;

public interface IRecipeRepository
{
    Task<IEnumerable<RecipeListingDTO>> GetRecipeListingsAsync(PageableParams pager);
    RecipeDTO? GetRecipeById(long id);
    Task<bool> RecipeExistsAsync(long id);
    void InsertRecipe(Recipe recipe);
    Task DeleteRecipe(long id);
    void UpdateRecipe(Recipe recipe);

    Task<IEnumerable<IngredientListingDTO>> GetIngredientListings(PageableParams pager);
    Task<IngredientDTO?> GetIngredient(long id);
    Ingredient InsertIngredient(IngredientDTO ingredient);
    void DeleteIngredient(long id);
    Ingredient UpdateIngredient(IngredientDTO ingredient);

    Task<IEnumerable<TagListingDTO>> GetTagListings(PageableParams pager);
    Task<TagDTO?> GetTag(long id);
    Tag InsertTag(TagDTO tag);
    void DeleteTag(long id);
    Tag UpdateTag(TagDTO tag);
}
