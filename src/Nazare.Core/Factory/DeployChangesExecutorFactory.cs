using Nazare.Core.Strategies;

namespace Nazare.Core.Factory
{
    internal sealed class DeployChangesExecutorFactory : IDeployChangesExecutorFactory
    {
        private readonly IEnumerable<IDeployChangesExecutor> _strategies;

        public IDeployChangesExecutor Create(string strategy)
        {
            return _strategies.First(s => s.Strategy == strategy);
        }
    }
}
