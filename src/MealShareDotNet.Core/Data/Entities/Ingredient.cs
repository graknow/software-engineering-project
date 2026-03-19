using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
[Table("Ingredients")]
public class Ingredient
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; set; }


    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = String.Empty;


    public long? ParentId { get; set; }

    public string? QuantityName { get; set; }

    /// <summary>Mass in tenths of a gram.</summary>
    public int? Mass { get; set; }

    /// <summary>Volume in tenths of a mL.</summary>
    public int? Volume { get; set; }

    public float? Quantity { get; set; }
}
