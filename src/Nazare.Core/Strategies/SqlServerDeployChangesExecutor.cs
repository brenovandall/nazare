using Microsoft.Data.SqlClient;
using Nazare.Core.Factory;
using Nazare.Core.Internal;

namespace Nazare.Core.Strategies
{
    internal sealed class SqlServerDeployChangesExecutor : IDeployChangesExecutor
    {
        private readonly IMigrationsHistoryService _migrationsHistoryService;
        private readonly IMigrationService _migrationService;

        public SqlServerDeployChangesExecutor(IMigrationsHistoryService migrationsHistoryService, IMigrationService migrationService)
        {
            _migrationsHistoryService = migrationsHistoryService;
            _migrationService = migrationService;
        }

        public string Strategy => DeployChangesProviders.SqlServer;

        public void Execute(DeployChanges deployChanges)
        {
            var verboseMode = deployChanges.Verbose;

            using var conn = new SqlConnection(deployChanges.ConnectionString);
            using var tc = conn.BeginTransaction();
            conn.Open();

            _migrationsHistoryService.EnsureMigrationsHistoryCreation(conn, verboseMode);
            _migrationService.Migrate(deployChanges, conn);

            tc.Commit();
            conn.Close();
        }

        public async Task ExecuteAsync(DeployChanges deployChanges, CancellationToken cancellationToken)
        {
            var verboseMode = deployChanges.Verbose;

            using var conn = new SqlConnection(deployChanges.ConnectionString);
            using var tc = conn.BeginTransaction();
            conn.Open();

            await _migrationsHistoryService.EnsureMigrationsHistoryCreationAsync(conn, verboseMode, cancellationToken);
            await _migrationService.MigrateAsync(deployChanges, conn, cancellationToken);

            tc.Commit();
            conn.Close();
        }
    }
}
