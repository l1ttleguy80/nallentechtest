using CsvHelper.Configuration;
using MeterReadingApi.Models;

namespace MeterReadingApi.Mappers
{
    public class MeterReadingsMap : ClassMap<MeterReadings>
	{
        public MeterReadingsMap()
        {
            Map(m => m.AccountId);
            Map(m => m.MeterReadingDateTime)
                .TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>()
                .TypeConverterOption.Format("dd/MM/yyyy HH:mm");
            Map(m => m.MeterReadValue);
        }
    }
}

