using Microsoft.Data.Sqlite;
using Dapper;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;

namespace MealShareDotNet.Core.Repositories;

public class SqliteMealPlanRepository :
    IMealPlanRepository
{
    /// <summary>
    /// Connection string to a Sqlite database.
    /// </summary>
    private readonly string _connectionString;

    private SqliteConnection _connection => new SqliteConnection(_connectionString);

    public SqliteMealPlanRepository(string connString)
    {
        _connectionString = connString;
    }

    public Task DeleteMealPlanAsync(long id)
    {
        var sql = "DELETE FROM MealPlans AS MP WHERE MP.Id = @Id;";

        using var conn = _connection;

        return conn.ExecuteAsync(sql, new { Id = id });
    }

    public Task<MealPlan?> GetMealPlanByIdAsync(long id)
    {
        var sql = """
            SELECT
                MP.Id,
                MP.RecipeId,
                MP.EventName,
                MP.ScheduledTime
            FROM MealPlans MP
            WHERE MP.Id = @Id
            """;

        using var conn = _connection;

        return conn.QuerySingleOrDefaultAsync<MealPlan>(sql, new { Id = id });
    }

    public Task<MealPlan> InsertMealPlanAsync(MealPlan meal)
    {
        var sql = """
            INSERT INTO MealPlans
            (RecipeId, EventName, ScheduledTime)
            VALUES
            (@RecipeId, @EventName, @ScheduledTime);
            SELECT * FROM MealPlans AS MP WHERE MP.Id = LAST_INSERT_ROWID();
            """;

        using var conn = _connection;

        return conn.QuerySingleAsync<MealPlan>(sql, meal);
    }

    public Task<IEnumerable<MealPlan>> SearchMealPlansAsync(GetMealPlansQuery query)
    {
        var sql = """
            SELECT
                MP.Id,
                MP.RecipeId,
                MP.EventName,
                MP.ScheduledTime,
                Recipe.Id AS RecipeId,
                Recipe.Name,
                Recipe.Instructions
            FROM MealPlans MP
            INNER JOIN Recipes as Recipe ON Recipe.Id = MP.RecipeId
            WHERE MP.ScheduledTime >= @Start AND MP.ScheduledTime <= @End;
            """;

        using var conn = _connection;

        return conn.QueryAsync<MealPlan, Recipe, MealPlan>(sql, (p, r) =>
        {
            p.Recipe = r;
            return p;
        },
        new
        {
            Start = query.Start.ToDateTime(TimeOnly.MinValue),
            End = query.End.ToDateTime(TimeOnly.MinValue),
        }, splitOn: "RecipeId");
    }

    public Task<MealPlan> UpdateMealPlanAsync(MealPlan meal)
    {
        var sql = """
            UPDATE MealPlans AS MP
            SET RecipeId = @RecipeId, EventName = @EventName, ScheduledTime = @ScheduledTime
            WHERE MP.Id = @Id;
            SELECT * FROM MealPlans AS MP WHERE MP.Id = @Id;
            """;

        using var conn = _connection;

        return conn.QuerySingleAsync<MealPlan>(sql, meal);
    }
}
