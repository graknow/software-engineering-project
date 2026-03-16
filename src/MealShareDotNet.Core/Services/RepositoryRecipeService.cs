using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Sqlite;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;
using MealShareDotNet.Core.Repositories;
using MealShareDotNet.Core.Services;

public class RepositoryRecipeService : IRecipeService
{
    private IRecipeRepository _db;

    public RepositoryRecipeService(IRecipeRepository db)
    {
        _db = db;
    }

    public async Task<IEnumerable<RecipeListingDTO>> GetRecipeListingsAsync(GetRecipeListingsQuery query)
    {
        var results = await _db.SearchRecipesAsync(null, query.PageSize, query.PageOffset);

        return results.Select(r => new RecipeListingDTO
                {
                    ID = r.ID,
                    Name = r.Name,
                    CookTime = r.CookTime,
                    ServingQuantity = r.ServingQuantity,
                    UpdatedDate = r.UpdatedDate,
                    Tags = []
                });
    }

    public async Task<RecipeDTO?> GetRecipeAsync(long id)
    {
        var result = await _db.GetRecipeByIdAsync(id);

        if (result is null)
        {
            return null;
        }

        return RecipeDTO.FromEntity(result);
    }

    public async Task<RecipeDTO> InsertRecipeAsync(RecipeDTO recipe)
    {
        ValidateOrThrow(recipe);

        try
        {
            await _db.InsertRecipeAsync(new() {
                    Name = recipe.Name,
                    CookTime = recipe.CookTime,
                    Price = recipe.Price,
                    ServingQuantity = recipe.ServingQuantity
                    });
        }
        catch (SqliteException)
        {
        }

        foreach (var ingredient in recipe.Ingredients)
        {
            _db.InsertIngredient(new() { Name = ingredient.Name });
        }

        return new();
    }

    public async Task<bool> DeleteRecipeAsync(long id)
    {
        if (!await _db.RecipeExistsAsync(id))
        {
            throw new KeyNotFoundException("ID doesn't exist in the database.");
        }

        try
        {
            // TODO: prevent deletion if included in a meal plan?
            await _db.DeleteRecipeAsync(id);

            if (_db is ITransactableRepository tdb)
            {
                tdb.Commit();
            }

            return true;
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"SQLException: {ex.Message}");

            if (_db is ITransactableRepository tdb)
            {
                tdb.Rollback();
            }

            return false;
        }
    }

    public async Task<RecipeDTO> UpdateRecipeAsync(RecipeDTO recipe)
    {
        return recipe;
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
