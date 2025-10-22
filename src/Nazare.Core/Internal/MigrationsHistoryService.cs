using System.Data;
using System.Data.Common;

namespace Nazare.Core.Internal
{
    internal interface IMigrationsHistoryService
    {
        void EnsureMigrationsHistoryCreation(IDbConnection connection, IDbTransaction transaction, DeployChanges deployChanges);
        Task EnsureMigrationsHistoryCreationAsync(IDbConnection connection, IDbTransaction transaction, DeployChanges deployChanges, CancellationToken cancellationToken);
    }

    internal sealed class MigrationsHistoryService : IMigrationsHistoryService
    {
        public void EnsureMigrationsHistoryCreation(IDbConnection connection, IDbTransaction transaction, DeployChanges deployChanges)
        {
            if (connection is DbConnection conn)
            {
                var verbose = deployChanges.Verbose;

                using var command = transaction is DbTransaction tc ? CreateTransactionCommand(conn, tc) : conn.CreateCommand();

                if (verbose)
                    Console.WriteLine("[BEGIN] Executing migrations creation batch.");

                command.CommandText = string.Join(" ",
                [
                    GetCreateTableScript(deployChanges.DatabaseProvider),
                    GetCreateIdxPversionPscript(deployChanges.DatabaseProvider),
                    GetCreateIdxPversion(deployChanges.DatabaseProvider)
                ]);

                command.ExecuteNonQuery();

                if (verbose)
                    Console.WriteLine("[END] Creation batch executed.");
            }
        }

        public async Task EnsureMigrationsHistoryCreationAsync(IDbConnection connection, IDbTransaction transaction, DeployChanges deployChanges, CancellationToken cancellationToken = default)
        {
            if (connection is DbConnection conn)
            {
                var verbose = deployChanges.Verbose;

                using var command = transaction is DbTransaction tc ? CreateTransactionCommand(conn, tc) : conn.CreateCommand();

                if (verbose)
                    Console.WriteLine("[BEGIN] Executing async migrations creation batch.");

                command.CommandText = string.Join(";",
                [
                    GetCreateTableScript(deployChanges.DatabaseProvider),
                    GetCreateIdxPversionPscript(deployChanges.DatabaseProvider),
                    GetCreateIdxPversion(deployChanges.DatabaseProvider)
                ]);

                await command.ExecuteNonQueryAsync(cancellationToken);

                if (verbose)
                    Console.WriteLine("[END] Async creation batch executed.");
            }
        }

        private static DbCommand CreateTransactionCommand(DbConnection connection, DbTransaction transaction)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;

            return command;
        }

        private static string GetCreateIdxPversionPscript(DatabaseProvider databaseProvider)
        {
            return databaseProvider switch
            {
                DatabaseProvider.SqlServer => @"
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'idx_pversion_pscript'
      AND object_id = OBJECT_ID('__nazaremigrationhistory')
)
BEGIN
    CREATE INDEX idx_pversion_pscript
    ON __nazaremigrationhistory (product_version, script_name);
END",
                _ => @"
CREATE INDEX IF NOT EXISTS idx_pversion_pscript
ON __nazaremigrationhistory (product_version, script_name)"
            };
        }

        private static string GetCreateIdxPversion(DatabaseProvider databaseProvider)
        {
            return databaseProvider switch
            {
                DatabaseProvider.SqlServer => @"
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'idx_pversion'
      AND object_id = OBJECT_ID('__nazaremigrationhistory')
)
BEGIN
    CREATE INDEX idx_pversion
    ON __nazaremigrationhistory (product_version);
END",
                _ => @"
CREATE INDEX IF NOT EXISTS idx_pversion
ON __nazaremigrationhistory (product_version)"
            };
        }

        private static string GetCreateTableScript(DatabaseProvider databaseProvider)
        {
            return databaseProvider switch
            {
                DatabaseProvider.SqlServer => @"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = '__nazaremigrationhistory')
BEGIN
    CREATE TABLE __nazaremigrationhistory(

        id bigint not null identity(1,1),
        product_version varchar(10) not null,
        script_name varchar(100) not null

        PRIMARY KEY(id)

    );
END",
                DatabaseProvider.Oracle => "",
                DatabaseProvider.Postgresql => "",
                DatabaseProvider.MySql => "",
                _ => @"
CREATE TABLE IF NOT EXISTS __nazaremigrationhistory(

    id bigint not null auto_increment,
    product_version varchar(10) not null,
    script_name varchar(100) not null

    PRIMARY KEY(id)

)",
            };
        }
    }
}
