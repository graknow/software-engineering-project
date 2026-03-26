using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
[Table("Tags")]
public class Tag : ICloneable
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; set; }


    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = String.Empty;

    public string? Description { get; set; }

    public object Clone()
    {
        return new Tag()
        {
            Id = Id,
            Name = (string)Name.Clone(),
            Description = (string?)Description?.Clone()
        };
    }
}
