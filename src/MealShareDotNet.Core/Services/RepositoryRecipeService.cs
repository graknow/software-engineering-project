using Microsoft.Data.Sqlite;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Repositories;
using MealShareDotNet.Core.Services;

public class RepositoryRecipeService : IRecipeService
{
    private IRecipeRepository _db;

    public RepositoryRecipeService(IRecipeRepository db)
    {
        _db = db;
    }

    public async Task<IEnumerable<RecipeListingDTO>> GetRecipeListingsAsync(uint? pageSize, uint? pageOffset)
    {
        var results = await _db.SearchRecipesAsync(null, pageSize, pageOffset);

        return results.Select(r => new RecipeListingDTO
                {
                    ID = r.ID,
                    Name = r.Name,
                    CookTime = r.CookTime,
                    ServingQuantity = r.ServingQuantity,
                    Tags = []
                });
    }

    public async Task<RecipeDTO?> GetRecipeAsync(long id)
    {
        var result = await _db.GetRecipeByIdAsync(id);

        return result is null ? null : new() {
            ID = result.ID,
            Name = result.Name,
        };
    }

    public async Task<RecipeDTO> InsertRecipeAsync(RecipeDTO recipe)
    {
        ValidateOrThrow(recipe);

        foreach (var ingredient in recipe.Ingredients)
        {
            var entity = new Ingredient()
            {
                Name = ingredient.Name,
            };

            _db.InsertIngredient(entity);
        }
    }

    public async Task<bool> DeleteRecipeAsync(long id)
    {
        try
        {
            // TODO: prevent deletion if included in a meal plan?
            await _db.DeleteRecipeAsync(id);
            return true;
        }
        catch (SqliteException)
        {
            return false;
        }
    }

    private void ValidateOrThrow(RecipeDTO recipe)
    {
        if (String.IsNullOrWhiteSpace(recipe.Name))
        {
            // I dont like exceptions
            throw new Exception("Recipe name is empty.");
        }
        else if (String.IsNullOrWhiteSpace(recipe.Instructions))
        {
            throw new Exception("Recipe instructions are empty.");
        }
    }
}
