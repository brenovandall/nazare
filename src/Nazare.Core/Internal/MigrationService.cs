using Nazare.Core.Extensions;
using System.Data;
using System.Data.Common;

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
                var files = GetScriptFiles(deployChanges);
                if (!files.Any())
                    return;

                var scripts = GetExecutedScripts(conn, tc);

                foreach (var file in files)
                {
                    var filename = Path.GetFileName(file);
                    if (scripts.Contains(filename) && FileNameChecker.CheckAndThrow(filename))
                    {
                        continue;
                    }

                    try
                    {
                        string sqlScript = File.ReadAllText(file);
                        string insertScript = GetInsertCommand(filename);

                        using var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandText = string.Join(";", [ sqlScript, insertScript ]);
                        command.Parameters.Add(CreateCustomParameter(command, "@version", "1"));
                        command.Parameters.Add(CreateCustomParameter(command, "@script", filename));
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
            // todo (some day haha)
            throw new NotImplementedException();
        }

        private static IEnumerable<string> GetScriptFiles(DeployChanges deployChanges)
        {
            var projFolder = Path.GetDirectoryName(deployChanges.ProjectPath);

            if (projFolder is null)
                throw new Exception("Folder is null...");

            var files = Directory
                .GetFiles(Path.Combine(projFolder, deployChanges.Path))
                .OrderBy(f => Path.GetFileName(f))
                .AsEnumerable();

            return files;
        }

        private static string GetInsertCommand(string filename)
        {
            if (filename.StartsWith("R__"))
                return "";

            return @"INSERT INTO __nazaremigrationhistory (product_version, script_name) 
                     VALUES (@version, @script)";
        }

        private static IDbDataParameter CreateCustomParameter(IDbCommand command, string paramName, object? value)
        {
            var param = command.CreateParameter();
            param.ParameterName = paramName;
            param.Value = value;

            return param;
        }

        private static HashSet<string> GetExecutedScripts(DbConnection connection, DbTransaction transaction)
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
