using CsvHelper;
using FluentAssertions;
using MeterReadingApi.CQRS;
using MeterReadingApi.Mappers;
using MeterReadingApi.Models;
using MeterReadingApi.Services.CsvService;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MeterReadingApiTests.CQRS
{
	public class MeterReadingsUploadRequestHandlerTests
	{
		private readonly Mock<ICsvService> csvService;
		private readonly MeterReadingsUploadRequestHandler resolver;

		public MeterReadingsUploadRequestHandlerTests()
		{
			this.csvService = new Mock<ICsvService>();
			this.resolver = new MeterReadingsUploadRequestHandler(this.csvService.Object);
		}

        [Fact]
        public async Task ShouldReturnSuccessCount()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            var data = new List<MeterReadings> {
                new MeterReadings
                {
                    AccountId = "AccountId",
                    MeterReadingDateTime = new DateTime(2023, 5, 1),
                    MeterReadValue = 1234
                }};

            this.csvService.Setup(x => x.ReadCsv<MeterReadings, MeterReadingsMap>(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(data.ToAsyncEnumerable());

            // Act
            var result = await this.resolver.Handle(new MeterReadingsUploadRequestHandler.Context(file.Object), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<MeterReadingsUpload>();
            result.As<MeterReadingsUpload>().SuccessCount.Should().Be(1);
        }

        [Fact]
        public async Task ShouldReturnFailureCount()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            var data = new List<MeterReadings> {
                new MeterReadings
                {
                    AccountId = "AccountId",
                    MeterReadingDateTime = new DateTime(2023, 5, 1),
                    MeterReadValue = 1234
                }};

            this.csvService.Setup(x => x.ReadCsv<MeterReadings, MeterReadingsMap>(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(data.ToAsyncEnumerable());

            // Act
            var result = await this.resolver.Handle(new MeterReadingsUploadRequestHandler.Context(file.Object), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<MeterReadingsUpload>();
            result.As<MeterReadingsUpload>().FailureCount.Should().Be(0);
        }

        [Fact]
		public void ShouldThrowArgumentExceptionOnHeaderValidationExceptionFromCsvService()
		{
            // Arrange
            var file = new Mock<IFormFile>();

            this.csvService.Setup(x => x.ReadCsv<MeterReadings, MeterReadingsMap>(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Throws(new HeaderValidationException(default, Array.Empty<InvalidHeader>()));

            // Act
            Func<Task> result = async () => await this.resolver.Handle(new MeterReadingsUploadRequestHandler.Context(file.Object), CancellationToken.None);

			// Assert
			result.Should().ThrowAsync<ArgumentException>().WithMessage("Invalid File Format");
		}
    }
}

