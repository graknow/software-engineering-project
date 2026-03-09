using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
public class Ingredient
{
    public long? ID { get; set; }

    public string Name { get; set; } = String.Empty;

    /// <summary>Mass in tenths of a gram.</summary>
    public int? Mass { get; set; }
    /// <summary>Volume in tenths of a mL.</summary>
    public int? Volume { get; set; }
    public float? Quantity { get; set; }
}
