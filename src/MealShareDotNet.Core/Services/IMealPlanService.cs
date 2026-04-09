using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Queries;

namespace MealShareDotNet.Core.Services;

public interface IMealPlanService
{
    Task<IEnumerable<MealPlanDTO>> GetMealPlansAsync(GetMealPlansQuery query);
    Task<IEnumerable<MealPlanDTO>> GetWeekMealPlansAsync(DateOnly startDate);
    Task<MealPlanDTO?> GetMealPlanAsync(long id);
    Task<MealPlanDTO> InsertMealPlanAsync(MealPlanDTO meal);
    Task DeleteMealPlanAsync(long id);
    Task<MealPlanDTO> UpdateMealPlanAsync(MealPlanDTO meal);
}
