using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace MeterReadingApi.Services.CsvService
{
    public class CsvService : ICsvService
	{
        public IAsyncEnumerable<T> ReadCsv<T, TMap>(Stream file, CancellationToken cancellationToken)
            where TMap : ClassMap
        {
            var reader = new StreamReader(file);
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TMap>();

            return csv.GetRecordsAsync<T>(cancellationToken);
        }
    }
}