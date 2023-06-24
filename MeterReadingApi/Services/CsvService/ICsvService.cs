using CsvHelper.Configuration;

namespace MeterReadingApi.Services.CsvService
{
    public interface ICsvService
    {
        IAsyncEnumerable<T> ReadCsv<T, TMap>(Stream file, CancellationToken cancellationToken)
            where TMap : ClassMap;
    }
}