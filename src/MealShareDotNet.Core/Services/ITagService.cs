using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Queries;

namespace MealShareDotNet.Core.Services;

public interface ITagService
{
    // TODO: Tag query params to include description
    Task<IEnumerable<TagListingDTO>> GetTagListingsAsync(GetTagListingsQuery query);
    Task<TagDTO?> GetTagAsync(long id);
    Task<TagDTO> InsertTagAsync(TagDTO tag);
    Task DeleteTagAsync(long id);
    Task<TagDTO> UpdateTagAsync(TagDTO tag);
}
