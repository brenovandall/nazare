using Nazare.Core.Strategies;

namespace Nazare.Core.Factory
{
    public interface IDeployChangesExecutorFactory
    {
        IDeployChangesExecutor Create(string strategy);
    }
}
