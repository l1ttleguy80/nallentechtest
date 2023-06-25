using FluentAssertions;
using MeterReadingApi.Models;
using MeterReadingApi.Services.MeterReadingService;
using MeterReadingApi.Services.SupplierDatabase;
using MeterReadingApi.Services.SupplierDatabase.Tables;
using MeterReadingApiTests.Mocks;
using Moq;

namespace MeterReadingApiTests.Services
{
    public class MeterReadingServiceTests
	{
        private readonly Mock<ISupplierDatabase> supplierDatabase;
		private readonly MeterReadingService meterReadingService;

        public MeterReadingServiceTests()
		{
			this.supplierDatabase = new Mock<ISupplierDatabase>();
			this.meterReadingService = new MeterReadingService(this.supplierDatabase.Object);
		}

		[InlineData(-1)]
        [InlineData(100000)]
        [Theory]
		public async Task ShouldReturnFalseForInvalidMeterReadingValue(int meterReadingValue)
		{
			// Arrange
			var meterReading = new MeterReading
			{
				AccountId = "accountId",
				MeterReadingDateTime = new DateTime(2023, 5, 1),
				MeterReadValue = meterReadingValue
			};

			// Act
			var result = await this.meterReadingService.AddMeterReading(meterReading, CancellationToken.None);

			// Assert
			result.Should().BeFalse();
			this.supplierDatabase.Verify(x => x.MeterReadings, Times.Never);
		}

		[Fact]
		public async Task ShouldReturnFalseIfMeterReadingAlreadyExistsInTheDatabase()
		{
            // Arrange
            var meterReading = new MeterReading
            {
                AccountId = "accountId",
                MeterReadingDateTime = new DateTime(2023, 5, 1),
                MeterReadValue = 12345
            };

			var meterReadingsList = new List<MeterReadings>
			{
				new MeterReadings
				{
					id = 1,
					AccountId = "accountId",
					MeterReadingDateTime = new DateTime(2023, 5, 1),
					MeterReadValue = "12345"
				}
			};

			this.supplierDatabase.Setup(x => x.MeterReadings).Returns(new MockDatabaseTable<MeterReadings>(meterReadingsList));

			// Act
			var result = await this.meterReadingService.AddMeterReading(meterReading, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            this.supplierDatabase.Verify(x => x.MeterReadings, Times.Once);
        }

        [Fact]
        public async Task ShouldReturnFalseIfDatabaseThrowsExceptionOnInsert()
        {
            // Arrange
            var meterReading = new MeterReading
            {
                AccountId = "accountId",
                MeterReadingDateTime = new DateTime(2023, 5, 1),
                MeterReadValue = 12345
            };

			var meterReadingsList = new List<MeterReadings>();

            this.supplierDatabase.Setup(x => x.MeterReadings).Returns(new MockDatabaseTable<MeterReadings>(meterReadingsList));

            // Act
            var result = await this.meterReadingService.AddMeterReading(meterReading, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            this.supplierDatabase.Verify(x => x.MeterReadings, Times.Exactly(2));
        }
    }
}

