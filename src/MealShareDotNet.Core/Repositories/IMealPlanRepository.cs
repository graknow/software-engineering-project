using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;

namespace MealShareDotNet.Core.Repositories;

public interface IMealPlanRepository
{
    // TODO: Add query object for searching by name/tag?
    public Task<IEnumerable<MealPlan>> SearchMealPlansAsync(GetMealPlansQuery query);
    public Task<MealPlan?> GetMealPlanByIdAsync(long id);
    public Task<MealPlan> InsertMealPlanAsync(MealPlan meal);
    public Task DeleteMealPlanAsync(long id);
    public Task<MealPlan> UpdateMealPlanAsync(MealPlan meal);
}
