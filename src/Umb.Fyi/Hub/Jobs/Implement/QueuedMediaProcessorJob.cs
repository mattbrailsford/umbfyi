using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using Umbraco.Cms.Core.Extensions;
using Umbraco.UIBuilder.Persistence;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Jobs.Implement
{
    public class QueuedMediaProcessorJob : RecuringJobBase
    {
        private IRepositoryFactory _repositoryFactory;
        private IWebHostEnvironment _env;

        public QueuedMediaProcessorJob(IRepositoryFactory repositoryFactory,
            IWebHostEnvironment env)
        {
            _repositoryFactory = repositoryFactory;
            _env = env;
        }

        public override Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            // Ensure temp DIR
            var tmpDir = _env.MapPathContentRoot("~/umbraco/UmbFyiQueue/");
            Directory.CreateDirectory(tmpDir);

            var files = Directory.GetFiles(tmpDir, "*.json", SearchOption.TopDirectoryOnly)
                .ToList();

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var items = JsonSerializer.Deserialize<MediaItem[]>(json);

                try
                {
                    if (items.Any())
                    {
                        using (var repo = _repositoryFactory.GetRepository<MediaItem, Guid>())
                        {
                            foreach (var item in items)
                            {
                                var existing = repo.GetCount(x => x.Link == item.Link);
                                if (existing.Success && existing.Model == 0)
                                {
                                    repo.Insert(item);
                                }
                            }
                        }
                    }

                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
                catch
                {
                    // If one file fails, keep trying others
                }
            }

            return Task.CompletedTask;
        }
    }
}
