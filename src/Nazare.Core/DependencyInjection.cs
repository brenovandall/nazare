using Microsoft.Extensions.DependencyInjection;
using Nazare.Core.Factory;
using Nazare.Core.Internal;
using Nazare.Core.Strategies;

namespace Nazare.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IDeployChangesExecutorFactory, DeployChangesExecutorFactory>();
            services.AddTransient<IDeployChangesExecutor, SqlServerDeployChangesExecutor>();

            services.AddScoped<IMigrationService, MigrationService>();

            return services;
        }
    }
}
