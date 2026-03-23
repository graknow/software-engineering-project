using System.Diagnostics.CodeAnalysis;
using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Data.DTOs;

[ExcludeFromCodeCoverage]
public class IngredientDTO
{
    public long? Id { get; set; }

    public string Name { get; set; } = String.Empty;

    public int? Mass { get; set; }
    public int? Volume { get; set; }
    public float? Quantity { get; set; }

    public static IngredientDTO FromEntity(RecipeIngredient ri)
    {
        return new()
        {
            Id = ri.Ingredient?.Id,
            Name = ri.Ingredient?.Name ?? String.Empty,
            Mass = ri.Mass,
            Volume = ri.Volume,
            Quantity = ri.Quantity
        };
    }
}

[ExcludeFromCodeCoverage]
public class IngredientListingDTO
{
    public long? Id { get; set; }

    public string Name { get; set; } = String.Empty;
}
