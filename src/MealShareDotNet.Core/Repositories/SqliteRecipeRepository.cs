using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;

namespace MealShareDotNet.Core.Repositories;

public class SqliteRecipeRepository :
    IRecipeRepository, ITransactableRepository, IDisposable
{
    /// <summary>
    /// Connection string to a Sqlite database.
    /// </summary>
    private readonly string _connectionString;

    private SqliteConnection? _connection;
    private SqliteConnection _activeConnection
    {
        get
        {
            if (_transaction is null)
            {
                throw new Exception("Can't use the active connection outside of a transaction.  Manage the transaction through the ITransactableRepository interface.");
            }

            if (_connection is not null)
            {
                return _connection;
            }

            _connection = new SqliteConnection(_connectionString);
            _connection.Open();

            return _connection;
        }
    }

    private IDbTransaction? _transaction;

    private int _isDisposed;

    public SqliteRecipeRepository(string connString)
    {
        _connectionString = connString;
    }

    public Task<IEnumerable<Recipe>> SearchRecipesAsync(GetRecipeListingsQuery query)
    {
        var sql = """
            SELECT
                Recipe.Id,
                Recipe.Name,
                Recipe.CookTime,
                Recipe.ServingQuantity,
                Recipe.UpdatedDate
            FROM Recipes Recipe
            """;

        if (query.PageSize is not null)
        {
            sql += "\nLIMIT @PageSize OFFSET @PageOffset";
        }

        sql += ";";

        using var queryConn = GetNewConnectionIfNecessary();
        var conn = queryConn is not null ? queryConn : _activeConnection;
        return conn.QueryAsync<Recipe>(sql,
                new
                {
                    PageSize = query.PageSize,
                    PageOffset = query.PageOffset
                }, _transaction);
    }

    public async Task<Recipe?> GetRecipeByIdAsync(long id)
    {
        var builder = new SqlBuilder();
        var sql = """
            SELECT
                Recipe.Id,
                Recipe.Name,
                Recipe.CookTime,
                Recipe.Price,
                Recipe.ServingQuantity,
                Recipe.Instructions,
                Recipe.UpdatedDate
            FROM Recipes Recipe
            WHERE Recipe.Id = @Id;

            SELECT
                Ingredient.Id,
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
            ) AS Ingredient ON Ingredient.Id = RI.IngredientId
            WHERE RI.RecipeId = @Id;

            SELECT
                Tag.Id,
                Tag.Name,
                Tag.Description
            FROM RecipeTag RT
            LEFT OUTER JOIN
            (
                SELECT
                    T.Id,
                    T.Name,
                    T.Description
                FROM Tags T
            ) AS Tag ON Tag.Id = RT.TagId
            WHERE RT.RecipeId = @Id;
            """;

        using var queryConn = GetNewConnectionIfNecessary();
        var conn = queryConn is not null ? queryConn : _activeConnection;
        using var results = conn.QueryMultiple(sql, new { Id = id });

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

    public Task<bool> RecipeExistsAsync(long id)
    {
        var sql = """
            SELECT
                COUNT(*)
            FROM Recipes Recipe
            WHERE Recipe.Id = @Id;
            """;

        using var queryConn = GetNewConnectionIfNecessary();
        var conn = queryConn is not null ? queryConn : _activeConnection;
        return conn.ExecuteScalarAsync<bool>(sql, new { Id = id });
    }

    public async Task<Recipe> InsertRecipeAsync(Recipe recipe)
    {
        Validator.ValidateObject(recipe, new ValidationContext(recipe));

        var recipeSql = """
            INSERT INTO Recipes
            (Name, CookTime, Price, ServingQuantity, Instructions)
            VALUES
            (@Name, @CookTime, @Price, @ServingQuantity, @Instructions);
            SELECT * FROM Recipes AS Recipe WHERE Recipe.Id = LAST_INSERT_ROWID();
            """;

        var riSql = """
            INSERT INTO RecipeIngredient
            (RecipeId, IngredientId, Mass, Volume, Quantity)
            VALUES
            (@RecipeId, @IngredientId, @Mass, @Volume, @Quantity);
            """;

        var rtSql = """
            INSERT INTO RecipeTag
            (RecipeId, IngredientId)
            VALUES
            (@RecipeId, @IngredientId);
            """;

        var result = await _activeConnection.QuerySingleAsync<Recipe>(recipeSql, new
        {
            Name = recipe.Name,
            CookTime = recipe.CookTime,
            Price = recipe.Price,
            ServingQuantity = recipe.ServingQuantity,
            Instructions = recipe.Instructions
        }, _transaction);

        var riTask = _activeConnection.ExecuteAsync(riSql, recipe.Ingredients, _transaction);
        var rtTask = _activeConnection.ExecuteAsync(rtSql, recipe.Tags, _transaction);

        await Task.WhenAll(riTask, rtTask);

        return result;
    }

    public Task DeleteRecipeAsync(long id)
    {
        var sql = """
            DELETE FROM RecipeIngredient AS RI WHERE RI.RecipeId = @Id;
            DELETE FROM RecipeTag AS RT WHERE RT.RecipeId = @Id;
            DELETE FROM Recipes AS Recipe WHERE Recipe.Id = @Id;
            """;

        return _activeConnection.ExecuteAsync(sql, new { Id = id }, _transaction);
    }

    public Recipe UpdateRecipe(Recipe recipe)
    {
        return new();
    }

    public Task<IEnumerable<Ingredient>> SearchIngredientsAsync(GetIngredientListingsQuery query)
    {
        var sql = """
            SELECT
                Ingredient.Id,
                Ingredient.Name
            FROM Ingredients Ingredient
            """;

        if (query.PageSize is not null)
        {
            sql += "\nLIMIT @PageSize OFFSET @PageOffset";
        }

        sql += ";";

        using var queryConn = GetNewConnectionIfNecessary();
        var conn = queryConn is not null ? queryConn : _activeConnection;
        return conn.QueryAsync<Ingredient>(sql, query);
    }

    public Task<Ingredient?> GetIngredientByIdAsync(long id)
    {
        var sql = """
            SELECT
                Ingredient.Id,
                Ingredient.Name
            FROM Ingredients Ingredient
            WHERE Ingredient.Id = @Id;
            """;

        using var queryConn = GetNewConnectionIfNecessary();
        var conn = queryConn is not null ? queryConn : _activeConnection;
        return conn.QuerySingleOrDefaultAsync<Ingredient>(sql, new { Id = id });
    }

    public async Task<Ingredient> InsertIngredientAsync(Ingredient ingredient)
    {
        var sql = """
            INSERT INTO Ingredients
            (Name)
            VALUES
            (@Name);
            SELECT * FROM Ingredients AS Ingredient WHERE Ingredient.Id = LAST_INSERT_ROWID();
            """;

        var entity = await _activeConnection.QuerySingleAsync<Ingredient>(sql, ingredient);

        return entity;
    }

    public Task DeleteIngredientAsync(long id)
    {
        var sql = """
            DELETE FROM RecipeIngredient AS RT WHERE RT.IngredientId = @Id;
            DELETE FROM Ingredients AS Ingredient WHERE Ingredient.Id = @Id;
            """;

        return _activeConnection.ExecuteAsync(sql, new { Id = id }, _transaction);
    }

    public Ingredient UpdateIngredient(Ingredient ingredient)
    {
        var sql = """
            UPDATE Ingredients AS Ingredient
            SET Name = @Name
            WHERE Ingredient.Id = @Id
            RETURNING *;
            """;

        using (var conn = _connection)
        {
            var entity = conn.ExecuteScalar<Ingredient>(sql, ingredient);

            return entity!;
        }
    }

    public Task<IEnumerable<Tag>> SearchTagsAsync(GetTagListingsQuery query)
    {
        var sql = """
            SELECT
                Tag.Id,
                Tag.Name,
                Tag.Description
            FROM Tags AS Tag
            """;

        if (query.PageSize is not null)
        {
            sql += "\nLIMIT @PageSize OFFSET @PageOffset";
        }

        sql += ";";

        using var queryConn = GetNewConnectionIfNecessary();
        var conn = queryConn is not null ? queryConn : _activeConnection;
        return conn.QueryAsync<Tag>(sql, query);
    }

    public Task<Tag?> GetTagByIdAsync(long id)
    {
        var sql = """
            SELECT
                Tag.Id,
                Tag.Name,
                Tag.Description
            FROM Tags Tag
            WHERE Tag.Id = @Id;
            """;

        using var queryConn = GetNewConnectionIfNecessary();
        var conn = queryConn is not null ? queryConn : _activeConnection;
        return conn.QuerySingleOrDefaultAsync<Tag>(sql, new { Id = id });
    }

    public async Task<Tag> InsertTagAsync(Tag tag)
    {
        var sql = """
            INSERT INTO Tags
            (Name, Description)
            VALUES
            (@Name, @Description);
            SELECT * FROM Tags AS Tag WHERE Tag.Id = LAST_INSERT_ROWID();
            """;

        return await _activeConnection.QuerySingleAsync<Tag>(sql, tag);
    }

    public Task DeleteTagAsync(long id)
    {
        var sql = """
            DELETE FROM RecipeTag AS RT WHERE RT.TagID = @Id;
            DELETE FROM Tags AS Tag WHERE Tag.ID = @Id;
            """;

        using (var conn = _connection)
        {
            return conn.ExecuteAsync(sql, new { Id = id }, _transaction);
        }
    }

    public Tag UpdateTag(Tag tag)
    {
        var sql = """
            UPDATE Tags AS Tag
            SET Name = @Name, Description = @Description
            WHERE Tag.Id = @Id
            RETURNING *;
            """;

        using (var conn = _connection)
        {
            var entity = conn.ExecuteScalar<Tag>(sql, tag);

            return entity!;
        }
    }

    public void BeginTransaction()
    {
        if (_transaction is not null)
        {
            // maybe throw an error for being an idiot.
            Rollback();
        }
        _connection = new SqliteConnection(_connectionString);
        _connection.Open();
        _transaction = _connection.BeginTransaction();
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
            _connection?.Dispose();
            _connection = null;
            _transaction?.Dispose();
            _transaction = null;
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
            _connection?.Dispose();
            _connection = null;
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) != 0)
        {
            return;
        }

        if (disposing)
        {
            _connection?.Dispose();
            _connection = null;
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    private IDbConnection? GetNewConnectionIfNecessary()
    {
        if (_connection is null)
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            return conn;
        }

        return null;
    }
}
