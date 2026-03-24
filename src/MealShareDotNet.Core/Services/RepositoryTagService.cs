using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;
using MealShareDotNet.Core.Repositories;

namespace MealShareDotNet.Core.Services;

public class RepositoryTagService : ITagService
{
    private readonly IRecipeRepository _db;

    public RepositoryTagService(IRecipeRepository _repository)
    {
        _db = _repository;
    }

    public async Task DeleteTagAsync(long id)
    {
        var transactable = _db as ITransactableRepository;

        try
        {
            transactable?.BeginTransaction();

            await _db.DeleteTagAsync(id);

            transactable?.Commit();
        }
        catch
        {
            transactable?.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<TagListingDTO>> GetTagListingsAsync(GetTagListingsQuery query)
    {
        var results = await _db.SearchTagsAsync(query);

        return results.Select(TagListingDTO.FromEntity).ToList();
    }

    public async Task<TagDTO?> GetTagAsync(long id)
    {
        var result = await _db.GetTagByIdAsync(id);

        if (result is null)
        {
            return null;
        }

        return TagDTO.FromEntity(result);
    }

    public async Task<TagDTO> InsertTagAsync(TagDTO tag)
    {
        var transactable = _db as ITransactableRepository;

        try
        {
            transactable?.BeginTransaction();

            var entity = DTOToEntity(tag);
            entity = await _db.InsertTagAsync(entity);

            transactable?.Commit();

            return TagDTO.FromEntity(entity);
        }
        catch
        {
            transactable?.Rollback();
            throw;
        }
    }

    public async Task<TagDTO> UpdateTagAsync(TagDTO tag)
    {
        var transactable = _db as ITransactableRepository;

        try
        {
            transactable?.BeginTransaction();

            var entity = DTOToEntity(tag);
            entity = await _db.UpdateTagAsync(entity);

            transactable?.Commit();

            return TagDTO.FromEntity(entity);
        }
        catch
        {
            transactable?.Rollback();
            throw;
        }
    }

    private Tag DTOToEntity(TagDTO dto)
    {
        return new Tag()
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description
        };
    }
}
