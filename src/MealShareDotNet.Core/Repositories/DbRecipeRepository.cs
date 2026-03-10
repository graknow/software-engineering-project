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

    public Task<IngredientDTO> GetIngredient(long id)
    {
        var sql = """
            SELECT
                Ingredient.ID,
                Ingredient.Name,
                Ingredient.Mass,
                Ingredient.Volume,
                Ingredient.Quantity
            FROM Ingredients Ingredient
            WHERE Ingredient.ID = @ID;
            """;

        using (var conn = _connection)
        {
            conn.Open();

            return conn.QuerySingleAsync<IngredientDTO>(sql, new { ID = id });
        }
    }

    public Task<IEnumerable<IngredientListingDTO>> GetIngredientListings(PageableParams pager)
    {
        var sql = """
            SELECT
                Ingredient.ID,
                Ingredient.Name
            FROM Ingredients Ingredient
            LIMIT @PageSize OFFSET @PageOffset;
            """;

        using (var conn = _connection)
        {
            conn.Open();

            return conn.QueryAsync<IngredientListingDTO>(sql, new
            {
                PageSize = pager.PageSize,
                PageOffset = pager.PageSize * (pager.PageNumber - 1)
            });
        }
    }

    public int InsertIngredients(IEnumerable<Ingredient> ingredients)
    {
        var sql = """
            INSERT INTO Ingredients
            (Name)
            VALUES
            (@Name);
            """;

        var rowsAffected = 0;

        using (var conn = _connection)
        {
            conn.Open();

            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    rowsAffected = conn.Execute(sql, ingredients);
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }
        }

        return rowsAffected;
    }

    public int DeleteIngredients(IEnumerable<long> ids)
    {
        var sql = """
            DELETE FROM Ingredients Ingredient WHERE Ingredient.ID = @ID;
            """;

        var rowsAffected = 0;

        using (var conn = _connection)
        {
            conn.Open();

            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    rowsAffected = conn.Execute(sql, ids.Select(id => new { ID = id }));
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }
        }

        return rowsAffected;
    }

    public void UpdateIngredient(Ingredient ingredient)
    {

    }

    public Task<TagDTO> GetTag(long id)
    {
        var sql = """
            SELECT
                Tag.ID,
                Tag.Name,
                Tag.Description
            FROM Tags Tag
            WHERE Tag.ID = @ID;
            """;

        using (var conn = _connection)
        {
            conn.Open();

            return conn.QuerySingleAsync<TagDTO>(sql, new { ID = id });
        }
    }

    public Task<IEnumerable<TagListingDTO>> GetTagListings(PageableParams pager)
    {
        var sql = """
            SELECT
                Tag.ID,
                Tag.Name,
                Tag.Description
            FROM Tags Tag
            LIMIT @PageSize OFFSET @PageOffset;
            """;

        using (var conn = _connection)
        {
            conn.Open();

            return conn.QueryAsync<TagListingDTO>(sql, new
            {
                PageSize = pager.PageSize,
                PageOffset = pager.PageSize * (pager.PageNumber - 1)
            });
        }
    }

    public int InsertTags(IEnumerable<Tag> tags)
    {
        var sql = """
            INSERT INTO Tags Tag
            (Name, Description)
            VALUES
            (@Name, @Description);
            """;

        var rowsAffected = 0;

        using (var conn = _connection)
        {
            conn.Open();

            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    rowsAffected = conn.Execute(sql, tags);
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }
        }

        return rowsAffected;
    }

    public int DeleteTags(IEnumerable<long> ids)
    {
        var sql = """
            DELETE FROM RecipeTag RT WHERE RT.TagID = @ID;
            DELETE FROM Tags Tag WHERE Tag.ID = @ID;
            """;

        var rowsAffected = 0;

        using (var conn = _connection)
        {
            conn.Open();

            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    rowsAffected = conn.Execute(sql, ids.Select(id => new { ID = id }));
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }
        }

        return rowsAffected;
    }

    public int UpdateTags(IEnumerable<Tag> tags)
    {
        var sql = """
            UPDATE Tags Tag
            SET Tag.Name = @Name, Tag.Description = @Description
            WHERE Tag.ID = @ID;
            """;

        var rowsAffected = 0;

        using (var conn = _connection)
        {
            conn.Open();

            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    rowsAffected = conn.Execute(sql, tags);
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }
        }

        return rowsAffected;
    }
}
