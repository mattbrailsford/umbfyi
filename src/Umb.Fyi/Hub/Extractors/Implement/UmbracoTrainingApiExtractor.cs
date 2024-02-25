using Hangfire.Server;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Umb.Fyi.Hub.Models;

namespace Umb.Fyi.Hub.Extractors.Implement
{
    public class UmbracoTrainingApiExtractor : MediaExtractorBase
    {
        private const string ApiEndpointUrl = "https://umbraco.com/umbraco/api/schedule/index";
        
        public UmbracoTrainingApiExtractor()
            : base(new[] { "hq", "event", "training" })
        { }

        public override async Task<IEnumerable<MediaItem>> ExtractMediaItemsAsync(PerformContext context = null, CancellationToken cancellationToken = default)
        {
            var raw = await FetchAsync(ApiEndpointUrl, cancellationToken);
            var json = JsonSerializer.Deserialize<UmbracoTrainingResponseDto>(raw, new JsonSerializerOptions()
            {
                AllowTrailingCommas = true
            });
            return json?.Dates?.SelectMany(x => x.Trainings)?.Select(x => new MediaItem
            {
                Link = x.RegistrationUrl + "?id=" + x.Id,
                Title = $"{x.Name}",
                Description = $"{x.Date} - {x.Location} - €{x.Price}",
                Date = DateTime.ParseExact(x.Date, "MMM dd, yyyy", CultureInfo.InvariantCulture).ToUniversalTime(),
                Source = "https://umbraco.com/training/book-courses/",
                Tags = Tags
            });
        }
    }

    public class UmbracoTrainingResponseDto
    {
        [JsonPropertyName("dates")]
        public IEnumerable<UmbracoTrainingDateDto> Dates { get; set; }
    }

    public class UmbracoTrainingDateDto
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("trainings")]
        public IEnumerable<UmbracoTrainingEntryDto> Trainings { get; set; }
    }

    public class UmbracoTrainingEntryDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("price")]
        public string Price { get; set; }

        [JsonPropertyName("registrationUrl")]
        public string RegistrationUrl { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }
    }
}
