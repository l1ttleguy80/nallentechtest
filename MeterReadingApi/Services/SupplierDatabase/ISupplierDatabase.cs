using LinqToDB;
using LinqToDB.Data;
using MeterReadingApi.Services.SupplierDatabase.Tables;

namespace MeterReadingApi.Services.SupplierDatabase
{
    public interface ISupplierDatabase
    {
        DataConnection DataConnection { get; }

        ITable<Customer> Customer { get; }

        ITable<MeterReadings> MeterReadings { get; }
    }
}

