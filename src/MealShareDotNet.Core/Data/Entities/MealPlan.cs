using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Entities;

[ExcludeFromCodeCoverage]
[Table("MealPlans")]
public class MealPlan : ICloneable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; set; }


    [Required]
    [ForeignKey("Recipe")]
    public long? RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    public string? EventName { get; set; }

    [Required]
    public DateTime ScheduledTime { get; set; }

    public object Clone()
    {
        var clone = new MealPlan()
        {
            Id = Id,
            RecipeId = RecipeId,
            EventName = EventName,
            ScheduledTime = ScheduledTime
        };

        return clone;
    }
}
