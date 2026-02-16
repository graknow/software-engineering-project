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
        using (var conn = _connection)
        {
            var sql = """
                SELECT
                    Recipe.ID,
                    Recipe.Name,
                    Recipe.CookTime,
                    Recipe.ServingQuantity
                FROM Recipes Recipe
                LIMIT @PageSize OFFSET @PageNumber
                """;

            return conn.QueryAsync<RecipeListingDTO>(sql, pager);
        }
    }

    public Task<Recipe> GetRecipeById(int id)
    {
        using (var conn = _connection)
        {
            var recipe_query = """
                SELECT *
                FROM Recipes Recipe
                INNER JOIN RecipeIngredient RI ON RI.RecipeID = Recipe.ID
                INNER JOIN RecipeTag RT ON RT.RecipeID = Recipe.ID
                """;

            var ingredients_query = """
                SELECT *
                FROM Recipe
                """;
            return conn.QuerySingleAsync<Recipe>(recipe_query, new { ID = id });
        }
    }

    public void InsertRecipe(Recipe recipe)
    {

    }

    public void DeleteRecipe(int id)
    {

    }

    public void UpdateRecipe(Recipe recipe)
    {

    }

    public void Save()
    {

    }
}
