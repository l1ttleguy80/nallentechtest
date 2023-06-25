using CsvHelper;
using FluentAssertions;
using MeterReadingApi.CQRS;
using MeterReadingApi.Mappers;
using MeterReadingApi.Models;
using MeterReadingApi.Services.CsvService;
using MeterReadingApi.Services.CustomerService;
using MeterReadingApi.Services.MeterReadingService;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MeterReadingApiTests.CQRS
{
    public class MeterReadingsUploadRequestHandlerTests
    {
        private readonly Mock<ICsvService> csvService;
        private readonly Mock<ICustomerService> customerService;
        private readonly Mock<IMeterReadingService> meterReadingService;
        private readonly MeterReadingsUploadRequestHandler resolver;

        public MeterReadingsUploadRequestHandlerTests()
        {
            this.csvService = new Mock<ICsvService>();
            this.customerService = new Mock<ICustomerService>();
            this.meterReadingService = new Mock<IMeterReadingService>();
            this.resolver = new MeterReadingsUploadRequestHandler(
            this.csvService.Object,
            this.customerService.Object,
            this.meterReadingService.Object);
        }

        [Fact]
        public async Task ShouldReturnSuccessCount()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            var data = new List<MeterReading> {
                new MeterReading
                {
                    AccountId = "AccountId",
                    MeterReadingDateTime = new DateTime(2023, 5, 1),
                    MeterReadValue = 1234
                }};

            this.csvService.Setup(x => x.ReadCsv<MeterReading, MeterReadingsMap>(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(data.ToAsyncEnumerable());

            this.customerService.Setup(x => x.AccountIdExists(It.Is<string>(y => y.Equals("AccountId")))).Returns(true);
            this.meterReadingService.Setup(x => x.AddMeterReading(It.IsAny<MeterReading>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

            // Act
            var result = await this.resolver.Handle(new MeterReadingsUploadRequestHandler.Context(file.Object), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<MeterReadingsUpload>();
            result.As<MeterReadingsUpload>().SuccessCount.Should().Be(1);
        }

        [Fact]
        public async Task ShouldReturnFailureIfCustomerDoesntExist()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            var data = new List<MeterReading> {
                new MeterReading
                {
                    AccountId = "AccountId",
                    MeterReadingDateTime = new DateTime(2023, 5, 1),
                    MeterReadValue = 1234
                }};

            this.csvService.Setup(x => x.ReadCsv<MeterReading, MeterReadingsMap>(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(data.ToAsyncEnumerable());

            this.customerService.Setup(x => x.AccountIdExists(It.Is<string>(y => y.Equals("AccountId")))).Returns(false);

            // Act
            var result = await this.resolver.Handle(new MeterReadingsUploadRequestHandler.Context(file.Object), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<MeterReadingsUpload>();
            result.As<MeterReadingsUpload>().FailureCount.Should().Be(1);
        }

        [Fact]
        public async Task ShouldReturnFailureIfMeterReadingServiceFailsToAddReading()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            var data = new List<MeterReading> {
                new MeterReading
                {
                    AccountId = "AccountId",
                    MeterReadingDateTime = new DateTime(2023, 5, 1),
                    MeterReadValue = 1234
                }};

            this.csvService.Setup(x => x.ReadCsv<MeterReading, MeterReadingsMap>(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(data.ToAsyncEnumerable());

            this.customerService.Setup(x => x.AccountIdExists(It.Is<string>(y => y.Equals("AccountId")))).Returns(true);

            this.meterReadingService.Setup(x => x.AddMeterReading(It.IsAny<MeterReading>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

            // Act
            var result = await this.resolver.Handle(new MeterReadingsUploadRequestHandler.Context(file.Object), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<MeterReadingsUpload>();
            result.As<MeterReadingsUpload>().FailureCount.Should().Be(1);
        }

        [Fact]
        public void ShouldThrowArgumentExceptionOnHeaderValidationExceptionFromCsvService()
        {
            // Arrange
            var file = new Mock<IFormFile>();

            this.csvService.Setup(x => x.ReadCsv<MeterReading, MeterReadingsMap>(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Throws(new HeaderValidationException(default, Array.Empty<InvalidHeader>()));

            // Act
            Func<Task> result = async () => await this.resolver.Handle(new MeterReadingsUploadRequestHandler.Context(file.Object), CancellationToken.None);

            // Assert
            result.Should().ThrowAsync<ArgumentException>().WithMessage("Invalid File Format");
        }
    }
}

