using Microsoft.Data.SqlClient;
using Nazare.Core.Factory;
using Nazare.Core.Internal;

namespace Nazare.Core.Strategies
{
    internal sealed class SqlServerDeployChangesExecutor : IDeployChangesExecutor
    {
        private readonly IMigrationsHistoryService migrationsHistoryService;

        public string Strategy => DeployChangesProviders.SqlServer;

        public void Execute(DeployChanges deployChanges)
        {
            var verboseMode = deployChanges.Verbose;

            using var conn = new SqlConnection(deployChanges.ConnectionString);
            conn.Open();

            migrationsHistoryService.EnsureMigrationsHistoryCreation(conn, deployChanges.Verbose);

            conn.Close();
        }

        public async Task ExecuteAsync(DeployChanges deployChanges, CancellationToken cancellationToken)
        {
            using var conn = new SqlConnection(deployChanges.ConnectionString);
            conn.Open();

            await migrationsHistoryService.EnsureMigrationsHistoryCreationAsync(conn, deployChanges.Verbose, cancellationToken);

            conn.Close();
        }
    }
}
