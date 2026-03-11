using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
public class Tag
{
    public long ID { get; set; }

    public string Name { get; set; } = String.Empty;
    public string? Description { get; set; }
}
