using Microsoft.Extensions.DependencyInjection;
using Nazare.Core;

internal class Program
{
    private static IServiceProvider _serviceProvider;

    private static void Main(string[] args)
    {
        RegisterServiceContainer();

        var builder = CreateBuilderByArgs(args);

        DisposeServiceContainer();
    }

    private static DeployChanges CreateBuilderByArgs(string[] args)
    {
        if (args.Length == 0 || args[0] != "run")
        {
            ShowUsage();
        }

        var builder = new DeployChanges();

        if (!args.Contains("--project"))
            builder.WithProjectPath(Directory.GetCurrentDirectory());

        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--project":
                    builder.WithProjectPath(Directory.GetCurrentDirectory() + args[i + 1]);
                    break;
                case "-c":
                    builder.WithConnectionString(args[i + 1]);
                    break;
                case "-p":
                    builder.WithScriptsPath(args[i + 1]);
                    break;
                case "-s":
                    builder.WithSchema(args[i + 1]);
                    break;
                case "-v":
                    builder.WithVerboseMode(true);
                    break;
            }
        }

        return builder;
    }

    private static void RegisterServiceContainer()
    {
        var collection = new ServiceCollection();

        collection.AddCoreServices();

        _serviceProvider = collection.BuildServiceProvider();
    }

    private static void DisposeServiceContainer()
    {
        if (_serviceProvider == null)
            return;

        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private static void ShowUsage()
    {
        Console.WriteLine("Usage: nazare run [--project] project path [-c] connection string [-p] scripts path [-s] schema [-v] verbose\n");
    }
}
