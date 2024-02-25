namespace Umb.Fyi.Hub.Jobs
{
    public abstract class RecuringJobBase
    {
        public abstract Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
