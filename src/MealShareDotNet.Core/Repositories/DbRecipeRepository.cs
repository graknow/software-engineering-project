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

    public Task<IEnumerable<RecipeListingDTO>> GetRecipeListingsAsync(PageableParams pager)
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

    public RecipeDTO? GetRecipeById(long id)
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
                var recipe = results.ReadSingleOrDefault<RecipeDTO>();
                if (recipe is null)
                {
                    return null;
                }

                recipe.Ingredients = results.Read<IngredientDTO>().ToList();
                recipe.Tags = results.Read<TagDTO>().ToList();
                return recipe;
            }
        }
    }

    public Task<bool> RecipeExistsAsync(long id)
    {
        var sql = """
            SELECT
                COUNT(*)
            FROM Recipes Recipe
            WHERE Recipe.ID = @ID;
            """;

        using (var conn = _connection)
        {
            return conn.ExecuteScalarAsync<bool>(sql, new { ID = id });
        }
    }

    public void InsertRecipe(Recipe recipe)
    {
        var sql = """
            INSERT INTO Recipes
            (Name, CookTime, Price, ServingQuantity, Instructions)
            VALUES
            (@Name, @CookTime, @Price, @ServingQuantity, @Instructions);
            """;

        using (var conn = _connection)
        {
            conn.Open();

            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var recipeTask = conn.ExecuteAsync(sql, new
                    {
                        Name = recipe.Name,
                        CookTime = recipe.CookTime,
                        Price = recipe.Price,
                        ServingQuantity = recipe.ServingQuantity,
                        Instructions = recipe.Instructions
                    });
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public Task DeleteRecipe(long id)
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
                    var deleteTask = conn.ExecuteAsync(sql, new { ID = id });
                    return deleteTask.ContinueWith(_ => trans.CommitAsync());
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public void UpdateRecipe(Recipe recipe)
    {

    }

    public Task<IngredientDTO?> GetIngredient(long id)
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

            return conn.QuerySingleOrDefaultAsync<IngredientDTO>(sql, new { ID = id });
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

    public Ingredient InsertIngredient(IngredientDTO ingredient)
    {
        var sql = """
            INSERT INTO Ingredients
            (Name)
            VALUES
            (@Name)
            RETURNING *;
            """;

        using (var conn = _connection)
        {
            conn.Open();

            var entity = conn.ExecuteScalar<Ingredient>(sql, ingredient);

            return entity!;
        }
    }

    public void DeleteIngredient(long id)
    {
        var sql = """
            DELETE FROM RecipeIngredient AS RT WHERE RT.IngredientID = @ID;
            DELETE FROM Ingredients AS Ingredient WHERE Ingredient.ID = @ID;
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

    public Ingredient UpdateIngredient(IngredientDTO ingredient)
    {
        var sql = """
            UPDATE Ingredients AS Ingredient
            SET Name = @Name
            WHERE Ingredient.ID = @ID
            RETURNING *;
            """;

        using (var conn = _connection)
        {
            conn.Open();

            var entity = conn.ExecuteScalar<Ingredient>(sql, ingredient);

            return entity!;
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

    public Task<TagDTO?> GetTag(long id)
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

            return conn.QuerySingleOrDefaultAsync<TagDTO>(sql, new { ID = id });
        }
    }

    public Tag InsertTag(TagDTO tag)
    {
        var sql = """
            INSERT INTO Tags
            (Name, Description)
            VALUES
            (@Name, @Description)
            RETURNING *;
            """;

        using (var conn = _connection)
        {
            conn.Open();

            var entity = conn.ExecuteScalar<Tag>(sql, tag);

            return entity!;
        }
    }

    public void DeleteTag(long id)
    {
        var sql = """
            DELETE FROM RecipeTag AS RT WHERE RT.TagID = @ID;
            DELETE FROM Tags AS Tag WHERE Tag.ID = @ID;
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
                    throw;
                }
            }
        }
    }

    public Tag UpdateTag(TagDTO tag)
    {
        var sql = """
            UPDATE Tags AS Tag
            SET Name = @Name, Description = @Description
            WHERE Tag.ID = @ID
            RETURNING *;
            """;

        using (var conn = _connection)
        {
            conn.Open();

            var entity = conn.ExecuteScalar<Tag>(sql, tag);

            return entity!;
        }
    }
}
