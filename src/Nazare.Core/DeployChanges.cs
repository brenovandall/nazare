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
            if (!File.Exists(projectPath))
                throw new Exception("net project file doesnt exists.");

            var extension = System.IO.Path.GetExtension(projectPath);
            if (string.IsNullOrWhiteSpace(extension))
                throw new Exception("net project must be provided.");
            else if (extension != ".csproj")
                throw new Exception("net project must be provided.");

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
            if (!Directory.Exists(path))
                throw new Exception("db scripts path must be a directory.");

            Path = path;
            return this;
        }

        public DeployChanges WithVerboseMode(bool active)
        {
            if (active)
                Console.WriteLine("[INFO] Verbose mode will be used.");

            Verbose = active;
            return this;
        }
    }
}
