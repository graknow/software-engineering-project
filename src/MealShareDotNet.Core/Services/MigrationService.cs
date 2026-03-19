using System.Reflection;
using System.Transactions;
using Microsoft.Data.Sqlite;
using Dapper;

namespace MealShareDotNet.Core.Services;

/// <summary>
/// Handles migrating a SQLite database according to the repository's migration
/// rules.
/// </summary>
public class MigrationService
{
    private class Migration
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
    }

    private const string EmbeddedResourcePrefix = "MealShareDotNet.Core";

    private readonly string _connectionString;
    private readonly string _migrationPrefix;

    private SqliteConnection _connection =>
        new SqliteConnection(_connectionString);

    public MigrationService(string connString, string migrationPath)
    {
        _connectionString = connString;
        _migrationPrefix = EmbeddedResourcePrefix + '.';
        _migrationPrefix += migrationPath.Replace(Path.PathSeparator, '.');
    }

    /// <summary>
    /// <para>
    /// Migrates a SQLite database according to the migrations in the specified
    /// migrationPath.
    /// </para>
    ///
    /// <para>
    /// Note: Creates the SQLite database file if it doesn't exist.
    /// </para>
    /// </summary>
    ///
    /// <returns>
    /// true: Migrations successfull.  false: Error in migration application.
    /// </returns>
    public bool Migrate()
    {
        var created = CreateDbIfNotExist();

        if (created)
        {
            Console.WriteLine("SQLite DB not found, new DB file created.");
        }

        IEnumerable<Migration> migrations = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceNames()
            .Select(MigrationFromResource)
            .Where(m => m is not null && !IsRollback(m.Name))
            .OrderBy(m => m!.Id)!;

        IEnumerable<Migration> appliedMigrations = [];

        using (var conn = _connection)
        {
            conn.Open();

            try
            {
                var sql = """
                    SELECT
                        Migration.Id,
                        Migration.Name
                    FROM Migrations Migration
                    ORDER BY Migration.ID ASC
                    """;

                appliedMigrations = conn.Query<Migration>(sql).ToList();
            }
            catch (SqliteException)
            {
                Console.WriteLine("Migration table not found: Running all migrations...");
            }
        }

        if ((appliedMigrations.LastOrDefault()?.Id ?? -1) + 1 != appliedMigrations.Count()
                || appliedMigrations.Count() > migrations.Count())
        {
            Console.WriteLine("Migration table is invalid.  Consider rebuilding the database.  Cancelling migrations...");

            return false;
        }

        if ((migrations.LastOrDefault()?.Id ?? -1) + 1 != migrations.Count())
        {
            Console.WriteLine("Migration directory is not valid.  Ids should be consecutive integers.  Cancelling migrations...");

            return false;
        }

        if (migrations.Count() == appliedMigrations.Count())
        {
            Console.WriteLine("Database is up to date.  No migrations needed.");
            return true;
        }

        Console.WriteLine("Beginning migrations...");

        using (var transaction = new TransactionScope())
        {
            foreach (var m in migrations.ToArray()[appliedMigrations.Count()..])
            {
                Console.WriteLine($"Applying migration Id: {m.Id} ({m.Name})...");

                try
                {
                    ExecuteMigration(m);
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine($"Error applying migration: {ex.Message}");

                    Console.WriteLine($"Cancelling migration transaction...");

                    return false;
                }

                RecordMigration(m);
            }

            transaction.Complete();
        }

        return true;
    }

    /// <summary>
    /// Creates the database file specified in the connection string if it doesn't already exist.
    /// </summary>
    ///
    /// <returns>
    /// true: The database file was created.  false: The database file already existed.
    /// </returns>
    public bool CreateDbIfNotExist()
    {
        var db_file = _connection.DataSource;

        if (!File.Exists(db_file))
        {
            File.Create(db_file);
            return true;
        }

        return false;
    }

    private void ExecuteMigration(Migration migration)
    {
        var sql = String.Empty;

        var migrationPath = GetMigrationResourceName(migration);

        using (var data = Assembly.GetExecutingAssembly().GetManifestResourceStream(migrationPath))
        {
            if (data is null)
            {
                throw new FileNotFoundException($"Embedded migration file \"{migrationPath}\" not found");
            }

            using (var r = new StreamReader(data))
            {
                sql = r.ReadToEnd();
            }
        }

        using (var conn = _connection)
        {
            conn.Open();
            conn.Execute(sql);
        }
    }

    private void RecordMigration(Migration migration)
    {
        var sql = """
            INSERT INTO Migrations (Id, Name) VALUES (@Id, @Name);
            """;

        using (var conn = _connection)
        {
            conn.Execute(sql, new { Id = migration.Id, Name = migration.Name });
        }
    }

    private Migration? MigrationFromResource(string resourceNameFull)
    {
        if (!resourceNameFull.StartsWith(_migrationPrefix))
        {
            return null;
        }

        var resourceName = Path
            .GetFileNameWithoutExtension(resourceNameFull[(_migrationPrefix.Length)..])
            .Trim('.');

        var nameIndex = resourceName.IndexOf('_');

        if (nameIndex <= 0)
        {
            Console.WriteLine($"Warning: Invalid migration file name found: {resourceName}. Skipping.");
            return null;
        }

        return new Migration
        {
            Id = int.Parse(resourceName[0..nameIndex]),
            Name = resourceName[(nameIndex + 1)..]
        };
    }

    private string GetMigrationResourceName(Migration m) => $"{_migrationPrefix}.{m.Id}_{m.Name}.sql";

    private bool IsRollback(string fileName) => fileName.EndsWith("_rollback");
}
