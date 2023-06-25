using Ardalis.GuardClauses;
using MeterReadingApi.Services.SupplierDatabase;

namespace MeterReadingApi.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly ISupplierDatabase supplierDatabase;

        public CustomerService(ISupplierDatabase supplierDatabase)
        {
            this.supplierDatabase = supplierDatabase;
        }

        public bool AccountIdExists(string accountId)
        {
            Guard.Against.NullOrWhiteSpace(accountId);

            return this.supplierDatabase.Customer.Any(c => c.AccountId == accountId);
        }
    }
}

