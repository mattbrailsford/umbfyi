using Hangfire;
using Umbraco.Extensions;

namespace Umb.Fyi.Hub.Jobs
{
    public class ServiceProviderJobActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderJobActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type) ?? _serviceProvider.CreateInstance(type);
        }
    }
}
