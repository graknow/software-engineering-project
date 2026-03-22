using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
[Table("Ingredients")]
public class Ingredient
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = String.Empty;

    public long? ParentId { get; set; }

    [NotMapped]
    public RecipeIngredient? RecipeIngredient { get; set; }

    [NotMapped]
    public long? RecipeId => RecipeIngredient?.RecipeId;
}
