using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Requests;

namespace MealShareDotNet.Core.Services;

public interface IRecipeService
{
    Task<IEnumerable<RecipeListingDTO>> GetRecipeListingsAsync(RecipeQueryParams query, PageableParams pager);
    Task<RecipeDTO> GetRecipeAsync(long id);
    Task<RecipeDTO> InsertRecipeAsync(RecipeDTO recipe);
    Task<bool> DeleteRecipeAsync(long id);
    Task<RecipeDTO> UpdateRecipeAsync(RecipeDTO recipe);
    // TODO: Patch?

    // TODO: really good comments should probably go here
    Task<IEnumerable<IngredientListingDTO>> GetIngredienntListingsAsync(string query, PageableParams pager);
    Task<IngredientDTO> GetIngredientAsync(long id);
    Task<IngredientDTO> InsertIngredientAsync(IngredientDTO ingredient);
    Task<bool> DeleteIngredientAsync(long id);
    Task<IngredientDTO> UpdateIngredientAsync(IngredientDTO ingredient);

    // TODO: Separate into separate services?
    // TODO: Tag query params to include description
    Task<IEnumerable<TagListingDTO>> GetTagListingsAsync(string query, PageableParams pager);
    Task<TagDTO> GetTagAsync(long id);
    Task<TagDTO> InsertTagAsync(TagDTO tag);
    Task<bool> DeleteTagAsync(long id);
    Task<TagDTO> UpdateTagAsync(TagDTO tag);
}
