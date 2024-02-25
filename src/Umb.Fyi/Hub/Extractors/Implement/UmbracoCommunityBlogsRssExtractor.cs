using System.Text.Json;
using System.Text.Json.Serialization;

namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class UmbracoCommunityBlogsRssExtractor : MultiRssMediaExtractorBase
    {
        private string communityBlogsListUrl = "https://raw.githubusercontent.com/umbraco/OurUmbraco/main/OurUmbraco.Site/config/CommunityBlogs.json";

        public override DateTime MinPubDate => DateTime.UtcNow.AddMonths(-3);
        // public override string[] FilterKeywords => new[] { "umbraco", "codecabin", "codegarden", "examine" };

        public UmbracoCommunityBlogsRssExtractor() 
            : base(new[] { "community", "blog" })
        { }

        public override async Task<string[]> GetFeedUrlsAsync(CancellationToken cancellationToken = default)
        {
            var raw = await FetchAsync(communityBlogsListUrl, cancellationToken);
            var json = JsonSerializer.Deserialize<UmbracoCommunityBlogs>(raw, new JsonSerializerOptions()
            {
                AllowTrailingCommas = true
            });
            return json?.Blogs?.Select(x => x.Rss).ToArray() ?? Array.Empty<string>();
        }
    }

    public class UmbracoCommunityBlogs
    {
        [JsonPropertyName("blogs")]
        public UmbracoCommunityBlog[] Blogs { get; set; }
    }

    public class UmbracoCommunityBlog
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("rss")]
        public string Rss { get; set; }
    }
}
