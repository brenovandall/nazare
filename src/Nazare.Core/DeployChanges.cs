namespace Nazare.Core
{
    public class DeployChanges
    {
        public string ProjectPath { get; private set; } = default!;
        public string ConnectionString { get; private set; } = default!;
        public string Schema { get; private set; } = default!;
        public string Path { get; private set; } = default!;
        public bool Verbose { get; private set; }

        public DeployChanges()
        {
            Verbose = false;
        }

        public DeployChanges WithProjectPath(string projectPath)
        {
            ProjectPath = projectPath;
            return this;
        }

        public DeployChanges WithConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
            return this;
        }

        public DeployChanges WithSchema(string schema)
        {
            Schema = schema;
            return this;
        }

        public DeployChanges WithScriptsPath(string path)
        {
            Path = path;
            return this;
        }

        public DeployChanges WithVerboseMode(bool active)
        {
            Verbose = active;
            return this;
        }
    }
}
