using MealShareDotNet.Core.Data.DTOs;

namespace MealShareDotNet.Core.Services;

public interface ITagService
{
    // TODO: Separate into separate services?
    // TODO: Tag query params to include description
    Task<IEnumerable<TagListingDTO>> GetTagListingsAsync(string query, uint? pageSize, uint? pageOffset);
    Task<TagDTO> GetTagAsync(long id);
    Task<TagDTO> InsertTagAsync(TagDTO tag);
    Task<bool> DeleteTagAsync(long id);
    Task<TagDTO> UpdateTagAsync(TagDTO tag);
}
