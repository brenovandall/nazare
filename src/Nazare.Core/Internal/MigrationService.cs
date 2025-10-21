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
        void Migrate(DeployChanges deployChanges, IDbConnection connection);
        Task MigrateAsync(DeployChanges deployChanges, IDbConnection connection, CancellationToken cancellationToken);
    }

    internal class MigrationService : IMigrationService
    {
        public void Migrate(DeployChanges deployChanges, IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        public Task MigrateAsync(DeployChanges deployChanges, IDbConnection connection, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
