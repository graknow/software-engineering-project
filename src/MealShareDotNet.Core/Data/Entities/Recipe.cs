using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
[Table("Recipes")]
public class Recipe : ICloneable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; set; }


    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = String.Empty;

    public int? CookTime { get; set; }

    public int? Price { get; set; }

    public int? ServingQuantity { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Instructions { get; set; } = String.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreationDate { get; init; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedDate { get; init; }


    // Joins
    [NotMapped]
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];

    [NotMapped]
    public ICollection<RecipeTag> RecipeTags { get; set; } = [];

    public object Clone()
    {
        var clone = new Recipe()
        {
            Id = Id,
            Name = Name,
            CookTime = CookTime,
            Price = Price,
            ServingQuantity = ServingQuantity,
            Instructions = (string)Instructions.Clone(),
            CreationDate = CreationDate,
            UpdatedDate = UpdatedDate,
        };

        foreach (var ri in RecipeIngredients)
        {
            var ingredient = (Ingredient?)ri.Ingredient?.Clone();

            clone.RecipeIngredients.Add(new()
            {
                Ingredient = ingredient,
                IngredientId = ingredient?.Id,
                Recipe = clone,
                RecipeId = clone.Id,
                Mass = ri.Mass,
                Volume = ri.Volume,
                Quantity = ri.Quantity
            });
        }

        foreach (var rt in RecipeTags)
        {
            var tag = (Tag?)rt.Tag?.Clone();

            clone.RecipeTags.Add(new()
            {
                Tag = tag,
                TagId = tag?.Id,
                Recipe = clone,
                RecipeId = clone.Id
            });
        }

        return clone;
    }
}
