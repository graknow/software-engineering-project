using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
[Table("RecipeTag")]
public class RecipeTag
{
    public Recipe Recipe { get; set; } = default!;

    public Tag Tag { get; set; } = default!;
}
