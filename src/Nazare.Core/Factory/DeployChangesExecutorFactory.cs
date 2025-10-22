using Nazare.Core.Common;
using Nazare.Core.Strategies;

namespace Nazare.Core.Factory
{
    internal class DeployChangesExecutorFactory : IFactory<IDeployChangesExecutor>, IDeployChangesExecutorFactory
    {
        private readonly IDictionary<string, Func<IDeployChangesExecutor>> _strategies;

        public DeployChangesExecutorFactory(IDictionary<string, Func<IDeployChangesExecutor>> strategies)
        {
            _strategies = strategies;
        }

        public IDeployChangesExecutor Create(string strategy)
        {
            if (!_strategies.TryGetValue(strategy, out var strat) || strat is null)
                throw new ArgumentOutOfRangeException(nameof(strategy), $"strat '{strategy}' is not registered");

            return strat();
        }
    }
}
