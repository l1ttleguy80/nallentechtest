namespace MeterReadingApi.Models
{
	public class MeterReadings
	{
		public required string AccountId { get; set; }

		public DateTime MeterReadingDateTime { get; set; }

		public int MeterReadValue { get; set; }
	}
}