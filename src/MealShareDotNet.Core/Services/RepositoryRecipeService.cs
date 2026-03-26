using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;
using MealShareDotNet.Core.Repositories;

namespace MealShareDotNet.Core.Services;

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

        var transactable = _db as ITransactableRepository;

        try
        {
            transactable?.BeginTransaction();

            // TODO: Proper async slop
            // TODO: Check ingredient/tag name similarities and raise exception if similar (Handled in client)
            var addIngredientTask = AddMissingIngredients(recipe);
            var addTagTask = AddMissingTags(recipe);

            await Task.WhenAll(addIngredientTask, addTagTask);

            var entity = DTOToEntity(recipe);

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

    public async Task DeleteRecipeAsync(long id)
    {
        var transactable = _db as ITransactableRepository;

        try
        {
            transactable?.BeginTransaction();

            // TODO: prevent deletion if included in a meal plan?
            await _db.DeleteRecipeAsync(id);

            transactable?.Commit();
        }
        catch
        {
            transactable?.Rollback();
            throw;
        }
    }

    public async Task<RecipeDTO> UpdateRecipeAsync(RecipeDTO recipe)
    {
        var transactable = _db as ITransactableRepository;

        try
        {
            transactable?.BeginTransaction();
            var addIngredientTask = AddMissingIngredients(recipe);
            var addTagTask = AddMissingTags(recipe);

            await Task.WhenAll(addIngredientTask, addTagTask);

            var entity = DTOToEntity(recipe);

            entity = await _db.UpdateRecipeAsync(entity);

            transactable?.Commit();

            return RecipeDTO.FromEntity(entity);
        }
        catch
        {
            transactable?.Rollback();
            throw;
        }
    }

    private async Task AddMissingIngredients(RecipeDTO recipe)
    {
        foreach (var ingredient in recipe.Ingredients.Where(i => i.Id is null))
        {
            var ingredientEntity = new Ingredient()
            {
                Name = ingredient.Name,
            };

            ingredientEntity = await _db.InsertIngredientAsync(ingredientEntity);

            ingredient.Id = ingredientEntity.Id;
        }
    }

    private async Task AddMissingTags(RecipeDTO recipe)
    {
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
    }

    private Recipe DTOToEntity(RecipeDTO recipe)
    {
        var entity = new Recipe()
        {
            Id = recipe.Id,
            Name = recipe.Name,
            CookTime = recipe.CookTime,
            Price = recipe.Price,
            ServingQuantity = recipe.ServingQuantity,
            Instructions = recipe.Instructions,
        };

        // TODO: perhaps move this logic to recipe repository?
        foreach (var ingredient in recipe.Ingredients)
        {
            var ri = new RecipeIngredient()
            {
                RecipeId = entity.Id,
                Recipe = entity,
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
                RecipeId = entity.Id,
                Recipe = entity,
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

        return entity;
    }
}
