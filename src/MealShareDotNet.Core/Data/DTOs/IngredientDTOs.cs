using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.DTOs;

[ExcludeFromCodeCoverage]
public class IngredientDTO
{
    public long ID { get; set; }

    public string Name { get; set; } = String.Empty;

    public int? Mass { get; set; }
    public int? Volume { get; set; }
    public float? Quantity { get; set; }
}

[ExcludeFromCodeCoverage]
public class IngredientNameDTO
{
    public long ID { get; set; }

    public string Name { get; set; } = String.Empty;
}
