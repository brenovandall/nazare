using Microsoft.Extensions.DependencyInjection;
using Nazare.Core.Extensions;
using Nazare.Core.Factory;
using Nazare.Core.Internal;
using Nazare.Core.Strategies;

namespace Nazare.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IMigrationsHistoryService, MigrationsHistoryService>();
            services.AddScoped<IMigrationService, MigrationService>();

            // factories
            services.InjectDeployChangesFactory();

            return services;
        }

        private static IServiceCollection InjectDeployChangesFactory(this IServiceCollection services)
        {
            services.AddTransient<SqlServerDeployChangesExecutor>();
            services.AddSingleton<IDeployChangesExecutorFactory>(ctx =>
            {
                var strats = new Dictionary<string, Func<IDeployChangesExecutor>>()
                {
                    [DeployChangesProviders.SqlServer] = () => ctx.GetRequiredService<SqlServerDeployChangesExecutor>()
                };

                return new DeployChangesExecutorFactory(strats);
            });

            return services;
        }
    }
}
