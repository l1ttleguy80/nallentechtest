using LinqToDB;
using MeterReadingApi.Models;
using MeterReadingApi.Services.SupplierDatabase;

namespace MeterReadingApi.Services.MeterReadingService
{
    public class MeterReadingService : IMeterReadingService
    {
        private readonly ISupplierDatabase supplierDatabase;

        public MeterReadingService(ISupplierDatabase supplierDatabase)
        {
            this.supplierDatabase = supplierDatabase;
        }

        public async Task<bool> AddMeterReading(MeterReading meterReading, CancellationToken cancellationToken)
        {
            if (meterReading.MeterReadValue < 0 || meterReading.MeterReadValue > 99999)
            {
                return false;
            }

            if (supplierDatabase.MeterReadings.Any(
                x => x.AccountId == meterReading.AccountId &&
                    x.MeterReadingDateTime == meterReading.MeterReadingDateTime &&
                    x.MeterReadValue == meterReading.MeterReadValue.ToString("00000")))
            {
                return false;
            }

            try
            {
                var result = await this.supplierDatabase.MeterReadings
                    .Value(x => x.AccountId, meterReading.AccountId)
                    .Value(x => x.MeterReadingDateTime, meterReading.MeterReadingDateTime)
                    .Value(x => x.MeterReadValue, meterReading.MeterReadValue.ToString("00000"))
                    .InsertAsync(cancellationToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

