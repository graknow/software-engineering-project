using Microsoft.Data.Sqlite;
using Dapper;
using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Repositories;

public class DbRecipeRepository : IRecipeRepository
{
    private readonly string _connectionString;

    private SqliteConnection _connection => new SqliteConnection(_connectionString);

    public DbRecipeRepository(string connString)
    {
        _connectionString = connString;
    }

    public IEnumerable<Recipe> GetRecipes()
    {
        return [];
    }

    public Recipe GetRecipeById(Guid id)
    {
        using (var conn = _connection)
        {
            return conn.QueryFirst<Recipe>("SELECT * FROM Recipe LIMIT 1");
        }
    }

    public void InsertRecipe(Recipe recipe)
    {

    }

    public void DeleteRecipe(Guid id)
    {

    }

    public void UpdateRecipe(Recipe recipe)
    {

    }

    public void Save()
    {

    }
}
