using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Repositories;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> SearchTagsAsync(
            string? query = null,
            uint? pageSize = null,
            uint? pageOffset = null
            );

    Task<Tag?> GetTagByIdAsync(long id);
    Tag InsertTag(Tag tag);
    Task DeleteTagAsync(long id);
    Tag UpdateTag(Tag tag);
}
