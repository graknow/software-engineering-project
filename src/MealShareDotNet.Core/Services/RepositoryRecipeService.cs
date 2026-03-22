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
        var entity = new Recipe()
        {
            Name = recipe.Name,
            CookTime = recipe.CookTime,
            Price = recipe.Price,
            ServingQuantity = recipe.ServingQuantity,
            Instructions = recipe.Instructions,
        };

        var transactable = _db as ITransactableRepository;

        try
        {
            transactable?.BeginTransaction();

            // TODO: Proper async slop
            // TODO: Check ingredient/tag name similarities and raise exception if similar (Handled in client)
            foreach (var ingredient in recipe.Ingredients.Where(i => i.Id is null))
            {
                var ingredientEntity = new Ingredient()
                {
                    Name = ingredient.Name,
                };

                ingredientEntity = await _db.InsertIngredientAsync(ingredientEntity);

                ingredient.Id = ingredientEntity.Id;
            }

            foreach (var tag in recipe.Tags.Where(t => t.Id is null))
            {
                var tagEntity = new Tag()
                {
                    Name = tag.Name,
                    Description = tag.Description
                };

                tagEntity = await _db.InsertTagAsync(tagEntity);

                tag.Id = tagEntity.Id;
            }

            // TODO: perhaps move this logic to recipe repository?
            foreach (var ingredient in recipe.Ingredients)
            {
                var ri = new RecipeIngredient()
                {
                    IngredientId = ingredient.Id,
                    Ingredient = new Ingredient()
                    {
                        Id = ingredient.Id,
                        Name = ingredient.Name
                    },
                    Mass = ingredient.Mass,
                    Volume = ingredient.Volume,
                    Quantity = ingredient.Quantity
                };

                entity.RecipeIngredients.Add(ri);
            }

            foreach (var tag in recipe.Tags)
            {
                var rt = new RecipeTag()
                {
                    TagId = tag.Id,
                    Tag = new Tag()
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        Description = tag.Description
                    }
                };

                entity.RecipeTags.Add(rt);
            }

            var result = await _db.InsertRecipeAsync(entity);

            transactable?.Commit();

            return RecipeDTO.FromEntity(result);
        }
        catch
        {
            transactable?.Rollback();
            throw;
        }
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
