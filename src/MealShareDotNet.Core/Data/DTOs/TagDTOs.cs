using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.DTOs;

[ExcludeFromCodeCoverage]
public class TagDTO
{
    public long ID { get; set; }

    public string Name { get; set; } = String.Empty;
    public string? Description { get; set; }
}

[ExcludeFromCodeCoverage]
public class TagListingDTO
{
    public long ID { get; set; }

    public string Name { get; set; } = String.Empty;
    public string? Description { get; set; }
}
