using Hangfire;
using Microsoft.AspNetCore.Builder;
using Umb.Fyi.Hub.Jobs;

namespace Umb.Fyi.Hub
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseUmbFyiHub(this IApplicationBuilder app)
        {
            GlobalConfiguration.Configuration.UseActivator(new ServiceProviderJobActivator(app.ApplicationServices));

            return app;
        }
    }
}
