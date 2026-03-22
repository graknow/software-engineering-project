using System.Diagnostics.CodeAnalysis;
using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Data.DTOs;

[ExcludeFromCodeCoverage]
public class RecipeListingDTO
{
    public long? Id { get; set; }

    public string Name { get; set; } = String.Empty;
    public int? CookTime { get; set; }
    public int? ServingQuantity { get; set; }
    public DateTime UpdatedDate { get; init; }

    public ICollection<TagDTO> Tags { get; set; } = [];

    public static RecipeListingDTO FromEntity(Recipe recipe)
    {
        return new()
        {
            Id = recipe.Id,
            CookTime = recipe.CookTime,
            ServingQuantity = recipe.ServingQuantity,
            UpdatedDate = recipe.UpdatedDate,
            Tags = recipe.RecipeTags
                .Select(rt => TagDTO.FromEntity(rt.Tag!)).ToList()
        };
    }
}

[ExcludeFromCodeCoverage]
public class RecipeDTO
{
    public long? Id { get; set; }

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
            Id = recipe.Id,
            Name = recipe.Name,
            CookTime = recipe.CookTime,
            Price = recipe.Price,
            ServingQuantity = recipe.ServingQuantity,
            Instructions = recipe.Instructions,
            UpdatedDate = recipe.UpdatedDate,
            Ingredients = recipe.RecipeIngredients
                .Select(ri => IngredientDTO.FromEntity(ri.Ingredient!)).ToList(),
            Tags = recipe.RecipeTags
                .Select(rt => TagDTO.FromEntity(rt.Tag!)).ToList()
        };
    }
}
