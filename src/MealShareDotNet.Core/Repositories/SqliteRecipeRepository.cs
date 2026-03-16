using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using Microsoft.Data.Sqlite;
using Dapper;
using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Repositories;

public class SqliteRecipeRepository :
    IRecipeRepository, ITransactableRepository, IDisposable
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

    private IDbTransaction? _transaction;
    private int _isDisposed;

    public SqliteRecipeRepository(string connString)
    {
        _connectionString = connString;
        _transaction = _connection.BeginTransaction();
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
                Recipe.ServingQuantity,
                Recipe.UpdatedDate
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
            return _transaction!.Connection!.ExecuteScalarAsync<bool>(sql, new { ID = id });
        }
    }

    public async Task<Recipe> InsertRecipeAsync(Recipe recipe)
    {
        Validator.ValidateObject(recipe, new ValidationContext(recipe));

        var sql = """
            INSERT INTO Recipes
            (Name, CookTime, Price, ServingQuantity, Instructions)
            VALUES
            (@Name, @CookTime, @Price, @ServingQuantity, @Instructions);
            """;

        var riSql = """
            INSERT INTO RecipeIngredient
            (RecipeID, IngredientID, Mass, Volume, Quantity)
            VALUES
            (@RecipeID, @IngredientID, @Mass, @Volume, @Quantity);
            """;

        var rtSql = """
            INSERT INTO RecipeTag
            (RecipeID, IngredientID)
            VALUES
            (@RecipeID, IngredientID);
            """;

        using var conn = _connection;

        var riTask = conn.ExecuteAsync(riSql, recipe.Ingredients);
        var rtTask = conn.ExecuteAsync(rtSql, recipe.Tags);

        await Task.WhenAll(riTask, rtTask);

        return await _transaction!.Connection!.QuerySingleAsync<Recipe>(sql, new
        {
            Name = recipe.Name,
            CookTime = recipe.CookTime,
            Price = recipe.Price,
            ServingQuantity = recipe.ServingQuantity,
            Instructions = recipe.Instructions
        });
    }

    public Task DeleteRecipeAsync(long id)
    {
        var sql = """
            DELETE FROM RecipeIngredient AS RI WHERE RI.RecipeID = @ID;
            DELETE FROM RecipeTag AS RT WHERE RT.RecipeID = @ID;
            DELETE FROM Recipes AS Recipe WHERE Recipe.ID = @ID;
            """;

        using (var conn = _connection)
        {
            return _transaction!.Connection!.ExecuteAsync(sql, new { ID = id }, _transaction);
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
            DELETE FROM Ingredients AS Ingredient WHERE Ingredient.ID = @ID;
            DELETE FROM RecipeIngredient AS RT WHERE RT.IngredientID = @ID;
            """;

        return _transaction!.Connection!.ExecuteAsync(sql, new { ID = id }, _transaction);
    }

    public Ingredient UpdateIngredient(Ingredient ingredient)
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
        {
            return conn.ExecuteAsync(sql, new { ID = id }, _transaction);
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

    public void Commit()
    {
        if (_transaction is null)
        {
            throw new Exception("No transaction to commit.");
        }

        try
        {
            _transaction.Commit();
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = _connection.BeginTransaction();
        }
    }

    public void Rollback()
    {
        if (_transaction is null)
        {
            throw new Exception("No transaction to rollback.");
        }

        try
        {
            _transaction.Rollback();
        }
        finally
        {
            _transaction.Dispose();
            _transaction = _connection.BeginTransaction();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }
    }
}
