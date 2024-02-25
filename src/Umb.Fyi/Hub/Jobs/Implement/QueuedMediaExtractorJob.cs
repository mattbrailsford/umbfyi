using Microsoft.Extensions.DependencyInjection;
using Umb.Fyi.Hub.Extractors;
using Microsoft.AspNetCore.Hosting;
using Umbraco.Cms.Core.Extensions;
using System.Text.Json;
using System.Text;

namespace Umb.Fyi.Hub.Jobs.Implement
{
    public class QueuedMediaExtractorJob<T> : RecuringJobBase
        where T : IMediaExtractor
    {
        private IServiceProvider _services;
        private IWebHostEnvironment _env;

        public QueuedMediaExtractorJob(IServiceProvider services,
            IWebHostEnvironment env)
        {
            _services = services;
            _env = env;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var extractor = _services.GetRequiredService<T>();
            var items = await extractor.ExtractMediaItemsAsync(null, cancellationToken);

            // Ensure temp DIR
            var tmpDir = _env.MapPathContentRoot("~/umbraco/UmbFyiQueue/");
            Directory.CreateDirectory(tmpDir);

            // Serialize to string
            var serializedItems = JsonSerializer.Serialize(items);

            // Write to disk
            File.WriteAllText($"{tmpDir}MI_{typeof(T).Name}_{DateTime.UtcNow:yyyyMMddHHmmssfff}.json", serializedItems, Encoding.UTF8);
        }
    }
}
