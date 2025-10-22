using Nazare.Core.Common;

namespace Nazare.Core.Strategies
{
    public interface IDeployChangesExecutor : IStrategy
    {
        string Strategy { get; }

        void Execute(DeployChanges deployChanges);
        Task ExecuteAsync(DeployChanges deployChanges, CancellationToken cancellationToken);
    }
}
