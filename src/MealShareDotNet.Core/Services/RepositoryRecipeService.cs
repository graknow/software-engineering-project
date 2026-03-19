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
        var results = await _db.SearchRecipesAsync(query);

        return results.Select(RecipeListingDTO.FromEntity);
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
            await _db.InsertIngredientAsync(new() { Name = ingredient.Name });
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
}
