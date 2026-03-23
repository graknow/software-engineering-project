using System.Diagnostics.CodeAnalysis;
using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Data.DTOs;

[ExcludeFromCodeCoverage]
public class TagDTO
{
    public long? Id { get; set; }

    public string Name { get; set; } = String.Empty;
    public string? Description { get; set; }

    public static TagDTO FromEntity(Tag tag)
    {
        return new()
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description
        };
    }
}

[ExcludeFromCodeCoverage]
public class TagListingDTO
{
    public long? Id { get; set; }

    public string Name { get; set; } = String.Empty;

    public static TagListingDTO FromEntity(Tag tag)
    {
        return new()
        {
            Id = tag.Id,
            Name = tag.Name,
        };
    }
}
