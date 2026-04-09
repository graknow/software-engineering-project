using System.Diagnostics.CodeAnalysis;
using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Data.DTOs;

[ExcludeFromCodeCoverage]
public class MealPlanListingDTO
{
    public long? Id { get; set; }

    public long? RecipeId { get; set; }
    public string? EventName { get; set; }
    public DateTime ScheduledTime { get; set; }

    public static MealPlanListingDTO FromEntity(MealPlan meal)
    {
        return new()
        {
            Id = meal.Id,
            EventName = meal.EventName,
            ScheduledTime = meal.ScheduledTime
        };
    }
}

[ExcludeFromCodeCoverage]
public class MealPlanDTO
{
    public long? Id { get; set; }

    public RecipeDTO Recipe { get; set; } = default!;
    public string? EventName { get; set; }
    public DateTime ScheduledTime { get; set; }

    public static MealPlanDTO FromEntity(MealPlan meal)
    {
        return new()
        {
            Id = meal.Id,
            Recipe = RecipeDTO.FromEntity(meal.Recipe ?? throw new ArgumentNullException("Recipe must not be null for MealPlanDTO.")),
            EventName = meal.EventName,
            ScheduledTime = meal.ScheduledTime
        };
    }
}
