using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Requests;

namespace MealShareDotNet.Core.Repositories;

public interface IRecipeRepository
{
    Task<IEnumerable<RecipeListingDTO>> GetRecipeListings(PageableParams pager);
    RecipeDTO GetRecipeById(long id);
    void InsertRecipe(Recipe recipe);
    void DeleteRecipe(long id);
    void UpdateRecipe(Recipe recipe);

    Task<IngredientDTO> GetIngredient(long id);
    Task<IEnumerable<IngredientListingDTO>> GetIngredientListings(PageableParams pager);
    int InsertIngredients(IEnumerable<Ingredient> ingredients);
    int DeleteIngredients(IEnumerable<long> ids);
    // update

    Task<TagDTO> GetTag(long id);
    Task<IEnumerable<TagListingDTO>> GetTagListings(PageableParams pager);
    int InsertTags(IEnumerable<Tag> tags);
    int DeleteTags(IEnumerable<long> ids);
    int UpdateTags(IEnumerable<Tag> tags);
}
