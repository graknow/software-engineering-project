using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
[Table("RecipeIngredient")]
public class RecipeIngredient
{
    [ForeignKey("Recipe")]
    public long? RecipeID { get; set; }
    public Recipe? Recipe { get; set; } = default!;

    [ForeignKey("Ingredient")]
    public long? IngredientID { get; set; }
    public Ingredient? Ingredient { get; set; } = default!;


    /// <summary>Mass in tenths of a gram.</summary>
    public int? Mass { get; set; }

    /// <summary>Volume in tenths of a mL.</summary>
    public int? Volume { get; set; }

    public float? Quantity { get; set; }
}
