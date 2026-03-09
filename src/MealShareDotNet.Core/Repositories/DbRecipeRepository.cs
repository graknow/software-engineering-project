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

    public RecipeDTO GetRecipeById(long id)
    {
        var sql = """
            SELECT
                Recipe.ID,
                Recipe.Name,
                Recipe.CookTime,
                Recipe.Price,
                Recipe.ServingQuantity,
                Recipe.Instructions,
                Recipe.UpdatedDate
            FROM Recipes Recipe
            WHERE Recipe.ID = @ID;

            SELECT
                Ingredient.ID,
                Ingredient.Name,
                RI.Mass,
                RI.Volume,
                RI.Quantity
            FROM RecipeIngredient RI
            LEFT OUTER JOIN
            (
                SELECT
                    I.ID,
                    I.Name
                FROM Ingredients I
            ) AS Ingredient ON Ingredient.ID = RI.IngredientID
            WHERE RI.RecipeID = @ID;

            SELECT
                Tag.ID,
                Tag.Name,
                Tag.Description
            FROM RecipeTag RT
            LEFT OUTER JOIN
            (
                SELECT
                    T.ID,
                    T.Name,
                    T.Description
                FROM Tags T
            ) AS Tag ON Tag.ID = RT.TagID
            WHERE RT.RecipeID = @ID;
            """;

        using (var conn = _connection)
        {
            conn.Open();

            using (var results = conn.QueryMultiple(sql, new { ID = id }))
            {
                var recipe = results.ReadSingle<RecipeDTO>();
                recipe.Ingredients = results.Read<IngredientDTO>().ToList();
                recipe.Tags = results.Read<TagDTO>().ToList();
                return recipe;
            }
        }

    }

    public void InsertRecipe(Recipe recipe)
    {
        var sql = """

            """;
    }

    public void DeleteRecipe(long id)
    {
        var sql = """
            DELETE FROM Recipes Recipe WHERE Recipe.ID = @ID;
            DELETE FROM RecipeIngredient RI WHERE RI.RecipeID = @ID;
            DELETE FROM RecipeTag RT WHERE RT.RecipeID = @ID;
            """;

        using (var conn = _connection)
        {
            conn.Open();

            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute(sql, new { ID = id });
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

    public void InsertIngredient(Ingredient ingredient)
    {

    }

    public void DeleteIngredient(long id)
    {
        throw new NotSupportedException();
    }

    public void UpdateIngredient(Ingredient ingredient)
    {

    }

    public void InsertTag(Tag tag)
    {
        if (tag.ID is not null)
        {

        }

        var sql = """
            INSERT INTO Tags Tag
            """;
    }

    public void DeleteTag(long id)
    {
        var sql = """
            DELETE FROM RecipeTag RT WHERE RT.TagID = @ID;
            DELETE FROM Tags Tag WHERE Tag.ID = @ID;
            """;

        using (var conn = _connection)
        {
            conn.Open();

            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute(sql, new { ID = id });
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }
        }
    }

    public void UpdateTag(Tag tag)
    {
    }

    public void Save()
    {

    }


}
