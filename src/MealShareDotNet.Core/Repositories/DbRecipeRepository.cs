using Microsoft.Data.Sqlite;
using Dapper;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Requests;

namespace MealShareDotNet.Core.Repositories;

public class DbRecipeRepository : IRecipeRepository
{
    private readonly string _connectionString;

    private SqliteConnection _connection => new SqliteConnection(_connectionString);

    public DbRecipeRepository(string connString)
    {
        _connectionString = connString;
    }

    public Task<IEnumerable<RecipeListingDTO>> GetRecipeListings(PageableParams pager)
    {
        var sql = """
            SELECT
            Recipe.ID,
                Recipe.Name,
                Recipe.CookTime,
                Recipe.ServingQuantity
            FROM Recipes Recipe
            LIMIT @PageSize OFFSET @PageOffset;
        """;

        using (var conn = _connection)
        {
            conn.Open();


            return conn.QueryAsync<RecipeListingDTO>(sql,
                    new
                    {
                        PageSize = pager.PageSize,
                        PageOffset = pager.PageSize * (pager.PageNumber - 1)
                    });
        }
    }

    public Task<RecipeDTO> GetRecipeById(long id)
    {
        var sql = """
            SELECT
                Recipe.*
                Ingredient.ID as IngredientID
                Ingredient.*
            FROM Recipes Recipe
            INNER JOIN RecipeIngredient RI ON RI.RecipeID = @ID
            INNER JOIN Ingredients Ingredient ON Ingredient.ID = RI.IngredientID
            INNER JOIN RecipeTag RT ON RT.RecipeID = @ID
            INNER JOIN Tags Tag ON Tag.ID = RT.TagID
            INNER JOIN Units Unit ON Unit.ID = RI.UnitID
            WHERE Recipe.ID = @ID
            """;

        using (var conn = _connection)
        {
            conn.Open();

            return conn.QuerySingleAsync<RecipeDTO>(sql);
        }

    }

    public void InsertRecipe(Recipe recipe)
    {

    }

    public void DeleteRecipe(long id)
    {
        using (var conn = _connection)
        {
            conn.Open();

            var action = """
                DELETE FROM Recipes Recipe WHERE Recipe.ID = @ID;
                DELETE FROM RecipeIngredient RI WHERE RI.RecipeID = @ID;
                DELETE FROM RecipeTag RT WHERE RT.RecipeID = @ID;
                """;

            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute(action, new { ID = id });
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }
        }
    }

    public void UpdateRecipe(Recipe recipe)
    {

    }

    public void Save()
    {

    }
}
