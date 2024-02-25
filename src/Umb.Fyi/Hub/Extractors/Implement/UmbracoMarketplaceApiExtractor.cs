using Hangfire.Server;
using System.Text.Json;
using System.Text.Json.Serialization;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class UmbracoMarketplaceApiExtractor : MediaExtractorBase
    {
        private const string ApiEndpointUrl = "https://api.marketplace.umbraco.com/api/v1.0/packages?orderBy=MostRecentlyCreated&fields=id,packageId,title,description,createdOn,lastPublishedOn,isHQ&pageSize=20&pageNumber=1";
        
        public DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3);

        public UmbracoMarketplaceApiExtractor()
            : base(new[] { "package", "marketplace" })
        { }

        public override async Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default)
        {
            var raw = await FetchAsync(ApiEndpointUrl, cancellationToken);
            var json = JsonSerializer.Deserialize<UmbracoPackgagesResponseDto>(raw, new JsonSerializerOptions()
            {
                AllowTrailingCommas = true
            });
            return json?.Results?.Where(x => x.CreatedOn > MinPubDate).Select(x => new MediaItem
            {
                Link = $"https://marketplace.umbraco.com/package/{x.PackageId.ToLowerInvariant()}",
                Title = x.PackageId,
                Description = x.Description,
                Date = x.CreatedOn.ToUniversalTime(),
                Source = "https://marketplace.umbraco.com",
                Tags = new[] { x.IsHQ ? "hq" : "community" }.Concat(Tags).ToArray()
            });
        }
    }

    public class UmbracoPackgagesResponseDto
    {
        [JsonPropertyName("results")]
        public IEnumerable<UmbracoPackageDto> Results { get; set; }

        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }
    }

    public class UmbracoPackageDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("packageId")]
        public string PackageId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("lastPublishedOn")]
        public DateTime LastPublishedOn { get; set; }

        [JsonPropertyName("isHQ")]
        public bool IsHQ { get; set; }
    }
}
