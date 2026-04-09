using System.Collections.Concurrent;
using System.ComponentModel.Design;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;
using MealShareDotNet.Core.Repositories;

namespace MealShareDotNet.Core.Services;

public class RepositoryMealPlanService : IMealPlanService
{
    private IMealPlanRepository _db;

    public RepositoryMealPlanService(IMealPlanRepository db)
    {
        _db = db;
    }

    public Task DeleteMealPlanAsync(long id)
    {
        return _db.DeleteMealPlanAsync(id);
    }

    public async Task<MealPlanDTO?> GetMealPlanAsync(long id)
    {
        var entity = await _db.GetMealPlanByIdAsync(id);

        if (entity is null)
        {
            return null;
        }

        return MealPlanDTO.FromEntity(entity);
    }

    public async Task<IEnumerable<MealPlanDTO>> GetMealPlansAsync(GetMealPlansQuery query)
    {
        var entities = await _db.SearchMealPlansAsync(query);

        return entities.Select(MealPlanDTO.FromEntity);
    }

    public async Task<IEnumerable<MealPlanDTO>> GetWeekMealPlansAsync(DateOnly startDate)
    {
        var query = new GetMealPlansQuery()
        {
            Start = startDate,
            End = startDate.AddDays(7)
        };

        var entities = await _db.SearchMealPlansAsync(query);

        return entities.Select(MealPlanDTO.FromEntity);
    }

    public async Task<MealPlanDTO> InsertMealPlanAsync(MealPlanDTO meal)
    {
        var entity = await _db.InsertMealPlanAsync(DTOToEntity(meal));

        return MealPlanDTO.FromEntity(entity);
    }

    public async Task<MealPlanDTO> UpdateMealPlanAsync(MealPlanDTO meal)
    {
        var entity = await _db.UpdateMealPlanAsync(DTOToEntity(meal));

        return MealPlanDTO.FromEntity(entity);
    }

    private MealPlan DTOToEntity(MealPlanDTO meal)
    {
        var entity = new MealPlan()
        {
            Id = meal.Id,
            RecipeId = meal.Recipe.Id,
            EventName = meal.EventName,
            ScheduledTime = meal.ScheduledTime
        };

        return entity;
    }
}
