using System.Reflection;
using Microsoft.Data.Sqlite;
using Dapper;

namespace MealShareDotNet.Core.Services;

public class MigrationService
{
    private readonly string _connectionString;
    private readonly string _migrationsDirectory;

    private SqliteConnection _connection => new SqliteConnection(_connectionString);

    public MigrationService(string connString, string migrationsDirectory)
    {
        _connectionString = connString;
        _migrationsDirectory = migrationsDirectory;
    }

    public bool CreateDbIfNotExist()
    {
        using (var conn = _connection)
        {
            conn.Open();

            var db_file = conn.DataSource;

            if (!File.Exists(db_file))
            {
                File.Create(db_file);
                return true;
            }

            return false;
        }
    }

    public bool Migrate()
    {
        string[] resNamesFull = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceNames();

        var resPrefix = String.Join('.', resNamesFull.FirstOrDefault()?.Split('.')[0..-2]
                ?? throw new Exception("No migrations found!"));

        var resNames = resNamesFull.Select(x => x.Split('.')[^2]);

        using (var conn = _connection)
        {
            conn.Open();

            var db_file = conn.DataSource;

            if (!File.Exists(db_file))
            {
                return false;
            }

            IEnumerable<string> newMigrations = [];
            var exists = conn.ExecuteScalar<bool>("SELECT COUNT() FROM Migrations LIMIT 1");

            if (!exists)
            {
                Assembly.GetExecutingAssembly().GetManifestResourceNames();

                newMigrations = Directory
                    .EnumerateFiles(_migrationsDirectory)
                    .Where(IsRollback);
            }
            else
            {
                var sql = """
                    SELECT
                        *
                    FROM Migrations Migration
                    ORDER BY Migration.ID DESC
                    LIMIT 1
                    """;

                var largestID = conn.ExecuteScalar<int>(sql);

                newMigrations = Directory
                    .EnumerateFiles(_migrationsDirectory)
                    .Where(IsRollback);
            }

            Console.WriteLine(String.Join(',', newMigrations));

            return true;
        }
    }

    private bool ExecuteMigration(string migrationPath)
    {
        return false;
    }

    private bool IsRollback(string fileName) => fileName.EndsWith("_rollback.sql");
}
