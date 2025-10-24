using Microsoft.Data.SqlClient;
using Nazare.Core.Extensions;
using Nazare.Core.Internal;

namespace Nazare.Core.Strategies
{
    internal class SqlServerDeployChangesExecutor : IDeployChangesExecutor
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
            using var conn = new SqlConnection(deployChanges.ConnectionString);
            conn.Open();

            using var tc = conn.BeginTransaction();

            _migrationsHistoryService.EnsureMigrationsHistoryCreation(conn, tc, deployChanges);
            _migrationService.Migrate(conn, tc, deployChanges);

            tc.Commit();
            conn.Close();
        }

        public async Task ExecuteAsync(DeployChanges deployChanges, CancellationToken cancellationToken)
        {
            using var conn = new SqlConnection(deployChanges.ConnectionString);
            await conn.OpenAsync(cancellationToken);

            using var tc = await conn.BeginTransactionAsync(cancellationToken);

            await _migrationsHistoryService.EnsureMigrationsHistoryCreationAsync(conn, tc, deployChanges, cancellationToken);
            await _migrationService.MigrateAsync(conn, tc, deployChanges, cancellationToken);

            await tc.CommitAsync(cancellationToken);
            await conn.CloseAsync();
        }
    }
}
