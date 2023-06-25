using FluentAssertions;
using MeterReadingApi.Services.CustomerService;
using MeterReadingApi.Services.SupplierDatabase;
using MeterReadingApi.Services.SupplierDatabase.Tables;
using MeterReadingApiTests.Mocks;
using Moq;

namespace MeterReadingApiTests.Services
{
	public class CustomerServiceTests
	{
		private readonly Mock<ISupplierDatabase> supplierDatabase;
        private readonly CustomerService customerService;

		public CustomerServiceTests()
		{
			this.supplierDatabase = new Mock<ISupplierDatabase>();
			this.customerService = new CustomerService(this.supplierDatabase.Object);
		}

		[InlineData("")]
		[InlineData(" ")]
		[Theory]
		public void ShouldThrowExceptionForEmptyAccountId(string accountId)
		{
            // Arrange
            // Act
            Action result = () => this.customerService.AccountIdExists(accountId);

			// Assert
			result.Should().ThrowExactly<ArgumentException>();
		}

        [Fact]
        public void ShouldThrowExceptionForNullAccountId()
        {
            // Arrange
            // Act
            Action result = () => this.customerService.AccountIdExists(null);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>();
        }

		[Fact]
		public void ShouldReturnTrueWhenAccountIdIsFound()
		{
			// Arrange
			var customerList = new List<Customer>
			{
				new Customer
				{
					AccountId = "accountId",
					FirstName = "firstName",
					LastName = "lastName"
				}
			};

			this.supplierDatabase.Setup(x => x.Customer).Returns(new MockDatabaseTable<Customer>(customerList));

			// Act
			var result = this.customerService.AccountIdExists("accountId");

			// Assert
			result.Should().BeTrue();
		}

        [Fact]
        public void ShouldReturnFalseWhenAccountIdIsNotFound()
        {
            // Arrange
            var customerList = new List<Customer>
            {
                new Customer
                {
                    AccountId = "accountId",
                    FirstName = "firstName",
                    LastName = "lastName"
                }
            };

            this.supplierDatabase.Setup(x => x.Customer).Returns(new MockDatabaseTable<Customer>(customerList));

            // Act
            var result = this.customerService.AccountIdExists("unknownAccountId");

            // Assert
            result.Should().BeFalse();
        }
    }
}
