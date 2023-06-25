using CsvHelper;
using MediatR;
using MeterReadingApi.Mappers;
using MeterReadingApi.Models;
using MeterReadingApi.Services.CsvService;
using MeterReadingApi.Services.CustomerService;
using MeterReadingApi.Services.MeterReadingService;

namespace MeterReadingApi.CQRS
{
    public class MeterReadingsUploadRequestHandler : IRequestHandler<MeterReadingsUploadRequestHandler.Context, MeterReadingsUpload>
    {
        private readonly ICsvService csvService;
        private readonly ICustomerService customerService;
        private readonly IMeterReadingService meterReadingService;

        public MeterReadingsUploadRequestHandler(ICsvService csvService, ICustomerService customerService, IMeterReadingService meterReadingService)
        {
            this.csvService = csvService;
            this.customerService = customerService;
            this.meterReadingService = meterReadingService;
        }

        public async Task<MeterReadingsUpload> Handle(Context request, CancellationToken cancellationToken)
        {
            var response = new MeterReadingsUpload();

            try
            {
                var readings = this.csvService.ReadCsv<MeterReading, MeterReadingsMap>(request.File.OpenReadStream(), cancellationToken);
                var records = await readings.ToListAsync(cancellationToken);

                foreach (var record in records)
                {
                    if (this.customerService.AccountIdExists(record.AccountId))
                    {
                        var success = await this.meterReadingService.AddMeterReading(record, cancellationToken);

                        if (success) response.SuccessCount++; else response.FailureCount++;
                    }
                    else
                    {
                        response.FailureCount++;
                    }
                }
            }
            catch (HeaderValidationException ex)
            {
                throw new ArgumentException("Invalid File Format", ex);
            }

            return response;
        }

        public class Context : IRequest<MeterReadingsUpload>
        {
            public Context(IFormFile file)
            {
                this.File = file;
            }

            public IFormFile File { get; }
        }
    }
}

