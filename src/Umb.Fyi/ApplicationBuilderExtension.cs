using Microsoft.AspNetCore.Builder;
using Umb.Fyi.Hub;

namespace Umb.Fyi
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseUmbFyi(this IApplicationBuilder app)
        {
            app.UseUmbFyiHub();

            return app;
        }
    }
}
