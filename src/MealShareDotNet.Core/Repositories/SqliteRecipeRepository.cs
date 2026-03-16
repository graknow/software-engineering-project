using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Data.Sqlite;
using Dapper;
using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Repositories;

public class SqliteRecipeRepository : IRecipeRepository
{
    private readonly string _connectionString;

    private SqliteConnection _connection
    {
        get
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }

    public SqliteRecipeRepository(string connString)
    {
        _connectionString = connString;
    }

    public Task<IEnumerable<Recipe>> SearchRecipesAsync(
            string? query,
            uint? pageSize,
            uint? pageOffset
            )
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
            return conn.QueryAsync<Recipe>(sql,
                    new
                    {
                        PageSize = pageSize,
                        PageOffset = pageOffset
                    });
        }
    }

    public async Task<Recipe?> GetRecipeByIdAsync(long id)
    {
        var builder = new SqlBuilder();
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
        using (var results = conn.QueryMultiple(sql, new { ID = id }))
        {
            // TODO: Proper async slop
            var recipe = await results.ReadSingleOrDefaultAsync<Recipe>();
            if (recipe is null)
            {
                return null;
            }

            recipe.Ingredients = (await results.ReadAsync<Ingredient>()).ToList();
            recipe.Tags = (await results.ReadAsync<Tag>()).ToList();
            return recipe;
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

    public Task<Recipe> InsertRecipeAsync(Recipe recipe)
    {
        Validator.ValidateObject(recipe, new ValidationContext(recipe));

        var baseSql = """
            INSERT INTO Recipes
            (Name, CookTime, Price, ServingQuantity, Instructions)
            VALUES
            (@Name, @CookTime, @Price, @ServingQuantity, @Instructions);
            """;

        var sql = new StringBuilder("INSERT INTO Recipes");



        using (var conn = _connection)
        using (var trans = conn.BeginTransaction())
        {
            foreach (var ingredient in recipe.Ingredients.Where(i => i.ID is not null))
            {
                ingredient.ID = InsertIngredient(ingredient).ID;
            }



            return conn.QuerySingleAsync<Recipe>(sql, new
            {
                Name = recipe.Name,
                CookTime = recipe.CookTime,
                Price = recipe.Price,
                ServingQuantity = recipe.ServingQuantity,
                Instructions = recipe.Instructions
            });
        }

    }

    public Task DeleteRecipeAsync(long id)
    {
        var sql = """
            DELETE FROM Recipes Recipe WHERE Recipe.ID = @ID;
            DELETE FROM RecipeIngredient RI WHERE RI.RecipeID = @ID;
            DELETE FROM RecipeTag RT WHERE RT.RecipeID = @ID;
            """;

        using (var conn = _connection)
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

    public Recipe UpdateRecipe(Recipe recipe)
    {
        return new();
    }

    public Task<IEnumerable<Ingredient>> SearchIngredientsAsync(
            string? query,
            uint? pageSize,
            uint? pageOffset
            )
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
            return conn.QueryAsync<Ingredient>(sql, new
            {
                PageSize = pageSize,
                PageOffset = pageOffset
            });
        }
    }

    public Task<Ingredient?> GetIngredientByIdAsync(long id)
    {
        var sql = """
            SELECT
                Ingredient.ID,
                Ingredient.Name
            FROM Ingredients Ingredient
            WHERE Ingredient.ID = @ID;
            """;

        using (var conn = _connection)
        {
            return conn.QuerySingleOrDefaultAsync<Ingredient>(sql, new { ID = id });
        }
    }

    public Ingredient InsertIngredient(Ingredient ingredient)
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
            var entity = conn.ExecuteScalar<Ingredient>(sql, ingredient);

            return entity!;
        }
    }

    public Task DeleteIngredientAsync(long id)
    {
        var sql = """
            DELETE FROM RecipeIngredient AS RT WHERE RT.IngredientID = @ID;
            DELETE FROM Ingredients AS Ingredient WHERE Ingredient.ID = @ID;
            """;

        using (var conn = _connection)
        using (var trans = conn.BeginTransaction())
        {
            try
            {
                var deleteTask = conn.ExecuteAsync(sql, new { ID = id });
                return deleteTask.ContinueWith(_ => trans.Commit());
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }
    }

    public Ingredient UpdateIngredient(Ingredient ingredient, long? recipeId = null)
    {
        var sql = """
            UPDATE Ingredients AS Ingredient
            SET Name = @Name
            WHERE Ingredient.ID = @ID
            RETURNING *;
            """;

        using (var conn = _connection)
        {
            var entity = conn.ExecuteScalar<Ingredient>(sql, ingredient);

            return entity!;
        }
    }

    public Task<IEnumerable<Tag>> SearchTagsAsync(
            string? query,
            uint? pageSize,
            uint? pageOffset
            )
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
            return conn.QueryAsync<Tag>(sql, new
            {
                PageSize = pageSize,
                PageOffset = pageOffset
            });
        }
    }

    public Task<Tag?> GetTagByIdAsync(long id)
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
            return conn.QuerySingleOrDefaultAsync<Tag>(sql, new { ID = id });
        }
    }

    public Tag InsertTag(Tag tag)
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
            var entity = conn.ExecuteScalar<Tag>(sql, tag);

            return entity!;
        }
    }

    public Task DeleteTagAsync(long id)
    {
        var sql = """
            DELETE FROM RecipeTag AS RT WHERE RT.TagID = @ID;
            DELETE FROM Tags AS Tag WHERE Tag.ID = @ID;
            """;

        using (var conn = _connection)
        using (var trans = conn.BeginTransaction())
        {
            try
            {
                var deleteTask = conn.ExecuteAsync(sql, new { ID = id });
                return deleteTask.ContinueWith(_ => trans.Commit());
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }
    }

    public Tag UpdateTag(Tag tag)
    {
        var sql = """
            UPDATE Tags AS Tag
            SET Name = @Name, Description = @Description
            WHERE Tag.ID = @ID
            RETURNING *;
            """;

        using (var conn = _connection)
        {
            var entity = conn.ExecuteScalar<Tag>(sql, tag);

            return entity!;
        }
    }
}
