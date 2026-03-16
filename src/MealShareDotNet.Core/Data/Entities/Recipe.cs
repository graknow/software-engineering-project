using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
[Table("Recipes")]
public class Recipe
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? ID { get; set; }


    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = String.Empty;

    public int? CookTime { get; set; }

    public int? Price { get; set; }

    public int? ServingQuantity { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Instructions { get; set; } = String.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreationDate { get; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedDate { get; }


    // Joins
    public ICollection<Ingredient> Ingredients { get; set; } = [];

    public ICollection<Tag> Tags { get; set; } = [];
}
