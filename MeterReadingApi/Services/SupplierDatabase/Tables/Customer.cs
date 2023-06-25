using LinqToDB.Mapping;

namespace MeterReadingApi.Services.SupplierDatabase.Tables
{
    [Table(Name = "customer", Schema = "public")]
	public class Customer
	{
		[NotNull]
		[PrimaryKey]
		[Column("accountid")]
		public required string AccountId { get; set; }

		[NotNull]
		[Column("firstname")]
		public required string FirstName { get; set; }

        [NotNull]
        [Column("lastname")]
        public required string LastName { get; set; }
	}
}

