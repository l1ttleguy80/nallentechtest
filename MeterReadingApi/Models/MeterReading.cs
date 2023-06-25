namespace MeterReadingApi.Models
{
	public class MeterReading
	{
		public required string AccountId { get; set; }

		public DateTime MeterReadingDateTime { get; set; }

		public int MeterReadValue { get; set; }
	}
}