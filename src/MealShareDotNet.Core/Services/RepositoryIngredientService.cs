using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;
using MealShareDotNet.Core.Repositories;

namespace MealShareDotNet.Core.Services;

public class RepositoryIngredientService : IIngredientService
{
    private readonly IRecipeRepository _db;

    public RepositoryIngredientService(IRecipeRepository _repository)
    {
        _db = _repository;
    }

    public async Task DeleteIngredientAsync(long id)
    {
        var transactable = _db as ITransactableRepository;

        try
        {
            transactable?.BeginTransaction();

            await _db.DeleteIngredientAsync(id);

            transactable?.Commit();
        }
        catch
        {
            transactable?.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<IngredientListingDTO>> GetIngredientListingsAsync(GetIngredientListingsQuery query)
    {
        var results = await _db.SearchIngredientsAsync(query);

        return results.Select(IngredientListingDTO.FromEntity).ToList();
    }

    public async Task<IngredientDTO?> GetIngredientAsync(long id)
    {
        var result = await _db.GetIngredientByIdAsync(id);

        if (result is null)
        {
            return null;
        }

        return IngredientDTO.FromEntity(result);
    }

    public async Task<IngredientDTO> InsertIngredientAsync(IngredientDTO ingredient)
    {
        var transactable = _db as ITransactableRepository;

        try
        {
            transactable?.BeginTransaction();

            var entity = DTOToEntity(ingredient);
            entity = await _db.InsertIngredientAsync(entity);

            transactable?.Commit();

            return IngredientDTO.FromEntity(entity);
        }
        catch
        {
            transactable?.Rollback();
            throw;
        }
    }

    public async Task<IngredientDTO> UpdateIngredientAsync(IngredientDTO ingredient)
    {
        var transactable = _db as ITransactableRepository;

        try
        {
            transactable?.BeginTransaction();

            var entity = DTOToEntity(ingredient);
            entity = await _db.UpdateIngredientAsync(entity);

            transactable?.Commit();

            return IngredientDTO.FromEntity(entity);
        }
        catch
        {
            transactable?.Rollback();
            throw;
        }
    }

    private Ingredient DTOToEntity(IngredientDTO dto)
    {
        return new Ingredient()
        {
            Id = dto.Id,
            Name = dto.Name,
            ParentId = 0
        };
    }
}
