using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using MeterReadingApi.Services.CsvService;

namespace MeterReadingApiTests.Services
{
    public class CsvServiceTests
	{
        private readonly CsvService csvService;

		public CsvServiceTests()
		{
            this.csvService = new CsvService();
		}

        [Fact]
		public async Task ShouldReturnMappedListForSuppliedStream()
		{
            // Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("Header1,Header2,Header3");
            writer.WriteLine("value1,value2,value3");
            writer.WriteLine("value4,,value5");
            writer.Flush();
            stream.Position = 0;

            // Act
            var result = await this.csvService.ReadCsv<TestModel, TestModelMap>(stream, CancellationToken.None).ToListAsync();

            // Assert
            result.Should().BeOfType<List<TestModel>>();
            result.Count.Should().Be(2);
            result.First().Header1.Should().Be("value1");
            result.First().Header2.Should().Be("value2");
            result.First().Header3.Should().Be("value3");
            result.Last().Header1.Should().Be("value4");
            result.Last().Header2.Should().Be(string.Empty); ;
            result.Last().Header3.Should().Be("value5");
        }


        [Fact]
        public async Task ShouldIgnoreColumnsNotInMapForSuppliedStream()
        {
            // Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("Header1,Header1a,Header2,Header2c,Header3");
            writer.WriteLine("value1,value1a,value2,value2c,value3");
            writer.WriteLine("value4,value4a,,value5c,value5");
            writer.Flush();
            stream.Position = 0;

            // Act
            var result = await this.csvService.ReadCsv<TestModel, TestModelMap>(stream, CancellationToken.None).ToListAsync();

            // Assert
            result.Should().BeOfType<List<TestModel>>();
            result.Count.Should().Be(2);
            result.First().Header1.Should().Be("value1");
            result.First().Header2.Should().Be("value2");
            result.First().Header3.Should().Be("value3");
            result.Last().Header1.Should().Be("value4");
            result.Last().Header2.Should().Be(string.Empty); ;
            result.Last().Header3.Should().Be("value5");
        }

        [Fact]
        public async Task ShouldReturnEmptyListWhenStreamIsEmpty()
        {
            // Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Flush();
            stream.Position = 0;

            // Act
            var result = await this.csvService.ReadCsv<TestModel, TestModelMap>(stream, CancellationToken.None).ToListAsync();

            // Assert
            result.Should().BeOfType<List<TestModel>>();
            result.Count.Should().Be(0);
        }

        [Fact]
        public void ShouldThrowHeaderValidationExceptionWhenStreamIsMissingExpectedHeaders()
        {
            // Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("InvaldiHeader1,InvalidHeader2,InvalidHeader3");
            writer.WriteLine("value1,value2,value2c,value3");
            writer.WriteLine("value4,value4a,,value5c,value5");
            writer.Flush();
            stream.Position = 0;

            // Act
            Func<Task> result = async () => await this.csvService.ReadCsv<TestModel, TestModelMap>(stream, CancellationToken.None).ToListAsync();

            // Assert
            result.Should().ThrowAsync<HeaderValidationException>();
        }

        internal class TestModel
        {
            public required string Header1 { get; set; }

            public required string Header2 { get; set; }

            public required string Header3 { get; set; }
        }

        internal class TestModelMap : ClassMap<TestModel>
        {
            public TestModelMap()
            {
                Map(m => m.Header1);
                Map(m => m.Header2);
                Map(m => m.Header3);
            }
        }
	}
}

