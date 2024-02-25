using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umb.Fyi.Hub.Models
{
    [TableName("fyiMediaTip")]
    [PrimaryKey("id", AutoIncrement = false)]
    [ExplicitColumns]
    public class MediaTip
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("link")]
        public string Link { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("source")]
        public string Source { get; set; }

        [Column("votes")]
        public int Votes { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        public MediaTip()
        {
            Id = RT.Comb.Provider.Sql.Create();
            Votes = 1;
            Date = DateTime.UtcNow;
        }
    }
}
