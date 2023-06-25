using MeterReadingApi.Models;

namespace MeterReadingApi.Services.MeterReadingService
{
    public interface IMeterReadingService
    {
        Task<bool> AddMeterReading(MeterReading meterReading, CancellationToken cancellationToken);
    }
}

