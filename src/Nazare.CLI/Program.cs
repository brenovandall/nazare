using Microsoft.Extensions.DependencyInjection;
using Nazare.Core;
using Nazare.Core.Extensions;
using Nazare.Core.Factory;

internal class Program
{
    private static IServiceProvider _serviceProvider;

    private static void Main(string[] args)
    {
        if (args.Length == 0 || args[0] != "run")
        {
            ShowUsage();
            return;
        }

        RegisterServiceContainer();

        var builder = CreateBuilderByArgs(args);
        var factory = GetDeployChangesExecutorFactory();
        var strategy = factory.Create(DeployChangesProviders.SqlServer);

        strategy.Execute(builder);

        DisposeServiceContainer();
    }

    private static DeployChanges CreateBuilderByArgs(string[] args)
    {
        var builder = new DeployChanges();
        bool containsProject = false;

        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--project":
                    builder.WithProjectPath(args[i + 1]);
                    containsProject = true;
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

        if (!containsProject)
            builder.WithProjectPath(Directory.GetCurrentDirectory());

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

    private static IDeployChangesExecutorFactory GetDeployChangesExecutorFactory()
    {
        var factory = _serviceProvider.GetService<IDeployChangesExecutorFactory>();

        if (factory == null)
            throw new InvalidOperationException("Null factory.");

        return factory;
    }

    private static void ShowUsage()
    {
        Console.WriteLine("Usage: nazare run [--project] project path [-c] connection string [-p] scripts path [-s] schema [-v] verbose\n");
    }
}
