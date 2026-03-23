using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
[Table("Tags")]
public class Tag
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; set; }


    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = String.Empty;

    public string? Description { get; set; }


    [NotMapped]
    public ICollection<RecipeTag> RecipeTags { get; set; } = [];
}
