using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
[Table("RecipeTag")]
public class RecipeTag
{
    [ForeignKey("Recipe")]
    public long? RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    [ForeignKey("Tag")]
    public long? TagId { get; set; }
    public Tag? Tag { get; set; }
}
