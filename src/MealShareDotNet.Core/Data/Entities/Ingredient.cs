using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
[Table("Ingredients")]
public class Ingredient : ICloneable
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = String.Empty;

    public long? ParentId { get; set; }

    public object Clone()
    {
        return new Ingredient()
        {
            Id = Id,
            Name = (string)Name.Clone(),
            ParentId = ParentId
        };
    }
}
