using System.Diagnostics.CodeAnalysis;
using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Data.DTOs;

[ExcludeFromCodeCoverage]
public class RecipeListingDTO
{
    public long? ID { get; set; }

    public string Name { get; set; } = String.Empty;
    public int? CookTime { get; set; }
    public int? ServingQuantity { get; set; }
    public DateTime UpdatedDate { get; init; }

    public ICollection<TagDTO> Tags { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public class RecipeDTO
{
    public long? ID { get; set; }

    public string Name { get; set; } = String.Empty;
    public int? CookTime { get; set; }
    public int? Price { get; set; }
    public int? ServingQuantity { get; set; }
    public string Instructions { get; set; } = String.Empty;
    public DateTime UpdatedDate { get; set; }

    public ICollection<IngredientDTO> Ingredients { get; set; } = [];
    public ICollection<TagDTO> Tags { get; set; } = [];

    public static RecipeDTO FromEntity(Recipe recipe)
    {
        return new()
        {
            ID = recipe.ID,
            Name = recipe.Name,
            CookTime = recipe.CookTime,
            Price = recipe.Price,
            ServingQuantity = recipe.ServingQuantity,
            Instructions = recipe.Instructions,
            UpdatedDate = recipe.UpdatedDate,
            Ingredients = recipe.Ingredients.Select(IngredientDTO.FromEntity).ToList(),
            Tags = recipe.Tags.Select(TagDTO.FromEntity).ToList()
        };
    }
}
