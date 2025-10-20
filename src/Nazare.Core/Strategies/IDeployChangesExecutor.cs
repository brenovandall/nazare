namespace Nazare.Core.Strategies
{
    public interface IDeployChangesExecutor
    {
        string Strategy { get; }

        void Execute(DeployChanges deployChanges);
        Task ExecuteAsync(DeployChanges deployChanges, CancellationToken cancellationToken);
    }
}
