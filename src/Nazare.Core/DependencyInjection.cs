using Microsoft.Extensions.DependencyInjection;
using Nazare.Core.Factory;
using Nazare.Core.Strategies;

namespace Nazare.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IDeployChangesExecutorFactory, DeployChangesExecutorFactory>();
            services.AddTransient<IDeployChangesExecutor, SqlServerDeployChangesExecutor>();

            return services;
        }
    }
}
