using NPoco;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umb.Fyi.Hub.Models
{
    [TableName("fyiMediaItem")]
    [PrimaryKey("id", AutoIncrement = false)]
    [ExplicitColumns]
    public class MediaItem
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("link")]
        public string Link { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("author")]
        public string Author { get; set; }

        [Column("source")]
        public string Source { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("tags")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonIgnore]
        public string T { get; set; }

        // [Ignore] 
        public string[] Tags
        {
            set => T = value != null && value.Length > 0 ? string.Join(",", value) : null;
            get => !string.IsNullOrWhiteSpace(T) ? T.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();
        }

        public MediaItem()
        { 
            Id = RT.Comb.Provider.Sql.Create();
        }
    }
}
