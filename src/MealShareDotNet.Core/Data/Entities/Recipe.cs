using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
public class Recipe
{
    public long? ID { get; set; }

    public string Name { get; set; } = String.Empty;
    public int? CookTime { get; set; }
    public int? Price { get; set; }
    public int? ServingQuantity { get; set; }
    public string Instructions { get; set; } = String.Empty;
    public DateTime CreationDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    // Joins
    public ICollection<Ingredient> Ingredients { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];
}
