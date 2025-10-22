using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nazare.Core.Internal
{
    internal interface IMigrationService
    {
        void Migrate(DeployChanges deployChanges, IDbConnection connection, IDbTransaction transaction);
        Task MigrateAsync(DeployChanges deployChanges, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken);
    }

    internal class MigrationService : IMigrationService
    {
        public void Migrate(DeployChanges deployChanges, IDbConnection connection, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task MigrateAsync(DeployChanges deployChanges, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
