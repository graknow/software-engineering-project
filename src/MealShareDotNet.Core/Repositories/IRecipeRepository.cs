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
    void Save();
}
