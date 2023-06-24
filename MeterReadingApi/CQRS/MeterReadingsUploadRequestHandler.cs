using CsvHelper;
using MediatR;
using MeterReadingApi.Mappers;
using MeterReadingApi.Models;
using MeterReadingApi.Services.CsvService;

namespace MeterReadingApi.CQRS
{
    public class MeterReadingsUploadRequestHandler : IRequestHandler<MeterReadingsUploadRequestHandler.Context, MeterReadingsUpload>
	{
        private readonly ICsvService csvService;

        public MeterReadingsUploadRequestHandler(ICsvService csvService)
		{
            this.csvService = csvService;
		}

        public async Task<MeterReadingsUpload> Handle(Context request, CancellationToken cancellationToken)
        {
            var response = new MeterReadingsUpload();

            try
            {
                var readings = this.csvService.ReadCsv<MeterReadings, MeterReadingsMap>(request.File.OpenReadStream(), cancellationToken);
                var records = await readings.ToListAsync(cancellationToken);
                response.SuccessCount = records.Count();
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

