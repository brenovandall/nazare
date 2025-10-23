using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nazare.Core.Internal
{
    internal interface IMigrationService
    {
        void Migrate(IDbConnection connection, IDbTransaction transaction, DeployChanges deployChanges);
        Task MigrateAsync(IDbConnection connection, IDbTransaction transaction, DeployChanges deployChanges, CancellationToken cancellationToken);
    }

    internal class MigrationService : IMigrationService
    {
        public void Migrate(IDbConnection connection, IDbTransaction transaction, DeployChanges deployChanges)
        {
            if (connection is DbConnection conn && transaction is DbTransaction tc)
            {
                var files = Directory
                    .GetFiles(deployChanges.Path)
                    .OrderBy(f => Path.GetFileName(f));

                if (!files.Any())
                    return;

                ISet<string> scripts = GetExecutedScripts(conn, tc);

                foreach (var file in files)
                {
                    var filename = Path.GetFileName(file);
                    if (scripts.Contains(filename))
                    {
                        continue;
                    }

                    try
                    {
                        var sqlScript = File.ReadAllText(file);

                        using var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandText = string.Join(";",
                            [
                                sqlScript,
                                @"INSERT INTO __nazaremigrationhistory (product_version, script_name) 
                                  VALUES (@version, @script)"
                            ]);
                        command.Parameters.Insert(0, 1);
                        command.Parameters.Insert(1, filename);

                        command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine(ex.ToString());
                        throw;
                    }
                }
            }
        }

        public Task MigrateAsync(IDbConnection connection, IDbTransaction transaction, DeployChanges deployChanges, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private static ISet<string> GetExecutedScripts(DbConnection connection, DbTransaction transaction)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "SELECT script_name FROM __nazaremigrationhistory";
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(reader.GetString(0));
            }

            return result;
        }
    }
}
