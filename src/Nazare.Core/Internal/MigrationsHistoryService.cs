using System.Data;
using System.Data.Common;

namespace Nazare.Core.Internal
{
    internal interface IMigrationsHistoryService
    {
        void EnsureMigrationsHistoryCreation(IDbConnection connection, bool verbose);
        Task EnsureMigrationsHistoryCreationAsync(IDbConnection connection, bool verbose, CancellationToken cancellationToken);
    }

    internal sealed class MigrationsHistoryService : IMigrationsHistoryService
    {
        public void EnsureMigrationsHistoryCreation(IDbConnection connection, bool verbose)
        {
            if (connection is DbConnection conn)
            {
                using var command = conn.CreateCommand();

                if (verbose)
                    Console.WriteLine("[BEGIN] Executing migrations creation batch.");

                command.CommandText = string.Join(";", new[]
                {
                    GetCreateTableScript(),
                    GetCreateIdxPversionPscript(),
                    GetCreateIdxPversion()
                });

                command.ExecuteNonQuery();

                if (verbose)
                    Console.WriteLine("[END] Creation batch executed.");
            }
        }

        public async Task EnsureMigrationsHistoryCreationAsync(IDbConnection connection, bool verbose, CancellationToken cancellationToken = default)
        {
            if (connection is DbConnection conn)
            {
                using var command = conn.CreateCommand();

                if (verbose)
                    Console.WriteLine("[BEGIN] Executing migrations creation batch.");

                command.CommandText = string.Join(";", new[]
                {
                    GetCreateTableScript(),
                    GetCreateIdxPversionPscript(),
                    GetCreateIdxPversion()
                });

                await command.ExecuteNonQueryAsync();

                if (verbose)
                    Console.WriteLine("[END] Creation batch executed.");
            }
        }

        private static string GetCreateIdxPversionPscript()
        {
            return @"
CREATE INDEX IF NOT EXISTS idx_pversion_pscript
ON __nazaremigrationhistory (product_version, script_name)";
        }

        private static string GetCreateIdxPversion()
        {
            return @"
CREATE INDEX IF NOT EXISTS idx_pversion
ON __nazaremigrationhistory (product_version)";
        }

        private static string GetCreateTableScript()
        {
            return @"
CREATE TABLE IF NOT EXISTS __nazaremigrationhistory(

    id bigint not null auto_increment,
    product_version varchar(10) not null,
    script_name varchar(100) not null

    PRIMARY KEY(id)

)";
        }
    }
}
