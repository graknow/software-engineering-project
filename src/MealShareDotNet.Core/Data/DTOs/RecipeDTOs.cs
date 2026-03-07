using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.DTOs;

[ExcludeFromCodeCoverage]
public class RecipeListingDTO
{
    public long ID { get; set; }

    public string Name { get; set; } = String.Empty;
    public int CookTime { get; set; }
    public int ServingQuantity { get; set; }

    public ICollection<TagDTO> Tags { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public class RecipeDTO
{
    public long ID { get; set; }

    public string Name { get; set; } = String.Empty;
    public int? CookTime { get; set; }
    public int? Price { get; set; }
    public int? ServingQuantity { get; set; }
    public string Instructions { get; set; } = String.Empty;
    public DateTime CreationDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    public ICollection<IngredientDTO> Ingredients { get; set; } = [];
    public ICollection<TagDTO> Tags { get; set; } = [];
}
