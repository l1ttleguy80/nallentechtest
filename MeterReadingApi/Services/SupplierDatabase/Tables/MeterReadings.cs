using LinqToDB.Mapping;

namespace MeterReadingApi.Services.SupplierDatabase.Tables
{
    [Table(Name = "meterreadings", Schema = "public")]
    public class MeterReadings
	{
        [NotNull]
        [PrimaryKey]
        [Column(IsIdentity = true), SequenceName("meterreadings_id_seq")]
        public int? id { get; set; }

        [Column("accountid")]
        public required string AccountId { get; set; }

        [Column("meterreadingdatetime")]
        public required DateTime MeterReadingDateTime { get; set; }

        [Column("meterreadvalue")]
        public required string MeterReadValue { get; set; }
    }
}

