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

    // TODO: Proper search parameters, like tags, ingredients, etc
    public Task<IEnumerable<Recipe>> SearchRecipesAsync(GetRecipeListingsQuery query)
    {
        var sql = """
            SELECT
                Recipe.Id,
                Recipe.Name,
                Recipe.CookTime,
                Recipe.ServingQuantity,
                Recipe.UpdatedDate
            FROM Recipes AS Recipe
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
                RI.RecipeId,
                RI.IngredientId,
                RI.Mass,
                RI.Volume,
                RI.Quantity
            FROM RecipeIngredient AS RI
            WHERE RI.RecipeId = @Id;

            SELECT
                Ingredient.Id,
                Ingredient.Name
            FROM RecipeIngredient AS RI
            LEFT OUTER JOIN
            (
                SELECT
                    I.Id,
                    I.Name
                FROM Ingredients I
            ) AS Ingredient ON Ingredient.Id = RI.IngredientId
            WHERE RI.RecipeId = @Id;

            SELECT
                RT.RecipeId,
                RT.TagId
            FROM RecipeTag AS RT
            WHERE RT.RecipeId = @Id;

            SELECT
                Tag.Id,
                Tag.Name,
                Tag.Description
            FROM RecipeTag AS RT
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

        recipe.RecipeIngredients = (await results.ReadAsync<RecipeIngredient>()).ToList();
        var ingredients = await results.ReadAsync<Ingredient>();
        recipe.RecipeTags = (await results.ReadAsync<RecipeTag>()).ToList();
        var tags = await results.ReadAsync<Tag>();

        foreach (var ingredient in ingredients)
        {
            var ri = recipe.RecipeIngredients.Single(ri => ri.IngredientId == ingredient.Id);

            ri.Ingredient = ingredient;
        }

        foreach (var tag in tags)
        {
            var rt = recipe.RecipeTags.Single(rt => rt.TagId == tag.Id);

            rt.Tag = tag;
        }

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
            (RecipeId, TagId)
            VALUES
            (@RecipeId, @TagId);
            """;

        var result = await _activeConnection.QuerySingleAsync<Recipe>(recipeSql, new
        {
            Name = recipe.Name,
            CookTime = recipe.CookTime,
            Price = recipe.Price,
            ServingQuantity = recipe.ServingQuantity,
            Instructions = recipe.Instructions
        }, _transaction);

        foreach (var ri in recipe.RecipeIngredients)
        {
            result.RecipeIngredients.Add(ri);
            ri.RecipeId = result.Id;
            ri.Recipe = result;
        }

        foreach (var rt in recipe.RecipeTags)
        {
            result.RecipeTags.Add(rt);
            rt.RecipeId = result.Id;
            rt.Recipe = result;
        }

        var riTask = _activeConnection.ExecuteAsync(riSql, result.RecipeIngredients, _transaction);
        var rtTask = _activeConnection.ExecuteAsync(rtSql, result.RecipeTags, _transaction);

        await Task.WhenAll(riTask, rtTask);

        return result;
    }

    public async Task DeleteRecipeAsync(long id)
    {
        var sql = """
            DELETE FROM RecipeIngredient AS RI WHERE RI.RecipeId = @Id;
            DELETE FROM RecipeTag AS RT WHERE RT.RecipeId = @Id;
            DELETE FROM Recipes AS Recipe WHERE Recipe.Id = @Id;
            """;

        var rowsAffected = await _activeConnection.ExecuteAsync(sql, new { Id = id }, _transaction);

        if (rowsAffected == 0)
        {
            throw new KeyNotFoundException($"Recipe Id \"{id}\" not found, deletion impossible");
        }
    }

    public async Task<Recipe> UpdateRecipeAsync(Recipe recipe)
    {
        Validator.ValidateObject(recipe, new ValidationContext(recipe));
        // TODO: validate tags and ingredients if anyone cares

        if (recipe.Id is null)
        {
            throw new ArgumentNullException();
        }

        // it is impossible for this to be null here but the compiler is still angry
        var entity = await GetRecipeByIdAsync(recipe.Id ?? -1);

        if (entity is null)
        {
            throw new KeyNotFoundException(recipe.Id.ToString());
        }

        // good time complexity sir
        IEnumerable<RecipeIngredient> delRis = entity.RecipeIngredients
            .ExceptBy<RecipeIngredient, long?>(
                    recipe.RecipeIngredients.Select(ri => ri.IngredientId),
                    ri => ri.IngredientId
                    );
        IEnumerable<RecipeTag> delRts = entity.RecipeTags
            .ExceptBy<RecipeTag, long?>(
                    recipe.RecipeTags.Select(rt => rt.TagId),
                    rt => rt.TagId
                    );

        var recipeSql = """
            UPDATE Recipes AS Recipe
            SET
                Name = @Name,
                CookTime = @CookTime,
                Price = @Price,
                ServingQuantity = @ServingQuantity,
                Instructions = @Instructions
            WHERE Recipe.Id = @Id;
            """;

        var riSql = """
            INSERT OR REPLACE INTO RecipeIngredient
            (RecipeId, IngredientId, Mass, Volume, Quantity)
            VALUES
            (@RecipeId, @IngredientId, @Mass, @Volume, @Quantity);
            """;

        var riDelSql = """
            DELETE FROM RecipeIngredient AS RI WHERE RI.RecipeId = @RecipeId AND RI.IngredientId = @IngredientId;
            """;

        var rtSql = """
            INSERT OR REPLACE INTO RecipeTag
            (RecipeId, TagId)
            VALUES
            (@RecipeId, @TagId);
            """;

        var rtDelSql = """
            DELETE FROM RecipeTag AS RT WHERE RT.RecipeId = @RecipeId AND RT.TagId = @TagId;
            """;

        var conn = _activeConnection;

        await conn.ExecuteAsync(recipeSql, recipe, _transaction);

        var riDeleteTask = conn.ExecuteAsync(riDelSql, delRis, _transaction);
        var riUpdateTask = conn.ExecuteAsync(riSql, recipe.RecipeIngredients, _transaction);
        var rtDeleteTask = conn.ExecuteAsync(rtDelSql, delRts, _transaction);
        var rtUpdateTask = conn.ExecuteAsync(rtSql, recipe.RecipeTags, _transaction);

        await Task.WhenAll(riUpdateTask, riDeleteTask, rtUpdateTask, rtDeleteTask);

        return await GetRecipeByIdAsync(recipe.Id ?? 0) ?? throw new Exception("Not throwable2");
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
        return conn.QueryAsync<Ingredient>(sql, query, _transaction);
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
        return conn.QuerySingleOrDefaultAsync<Ingredient>(sql, new { Id = id }, _transaction);
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

    public async Task DeleteIngredientAsync(long id)
    {
        var sql = """
            DELETE FROM RecipeIngredient AS RT WHERE RT.IngredientId = @Id;
            DELETE FROM Ingredients AS Ingredient WHERE Ingredient.Id = @Id;
            """;

        var rowsAffected = await _activeConnection.ExecuteAsync(sql, new { Id = id }, _transaction);

        if (rowsAffected == 0)
        {
            throw new KeyNotFoundException($"Ingredient Id \"{id}\" not found, deletion impossible");
        }
    }

    public Task<Ingredient> UpdateIngredientAsync(Ingredient ingredient)
    {
        Validator.ValidateObject(ingredient, new ValidationContext(ingredient));

        var sql = """
            UPDATE Ingredients AS Ingredient
            SET Name = @Name
            WHERE Ingredient.Id = @Id;
            SELECT * FROM Ingredients AS Ingredient WHERE Ingredient.Id = @Id;
            """;

        return _activeConnection.QuerySingleAsync<Ingredient>(sql, ingredient, _transaction);
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
        return conn.QueryAsync<Tag>(sql, query, _transaction);
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
        return conn.QuerySingleOrDefaultAsync<Tag>(sql, new { Id = id }, _transaction);
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

        return await _activeConnection.QuerySingleAsync<Tag>(sql, tag, _transaction);
    }

    public async Task DeleteTagAsync(long id)
    {
        var sql = """
            DELETE FROM RecipeTag AS RT WHERE RT.TagID = @Id;
            DELETE FROM Tags AS Tag WHERE Tag.ID = @Id;
            """;

        var rowsAffected = await _activeConnection.ExecuteAsync(sql, new { Id = id }, _transaction);

        if (rowsAffected == 0)
        {
            throw new KeyNotFoundException($"Tag Id \"{id}\" not found, deletion impossible");
        }
    }

    public Task<Tag> UpdateTagAsync(Tag tag)
    {
        var sql = """
            UPDATE Tags AS Tag
            SET Name = @Name, Description = @Description
            WHERE Tag.Id = @Id;
            SELECT * FROM Tags AS Tag WHERE Tag.Id = @Id;
            """;

        return _activeConnection.QuerySingleAsync<Tag>(sql, tag, _transaction);
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
