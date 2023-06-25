using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using MeterReadingApi.Services.SupplierDatabase.Tables;
using Npgsql;

namespace MeterReadingApi.Services.SupplierDatabase
{
    public class SupplierDatabase : DataConnection, ISupplierDatabase
    {
        public SupplierDatabase(string connectionString)
        : base(new PostgreSQLDataProvider("Postgressql", PostgreSQLVersion.v95), new NpgsqlConnection(connectionString))
        {
        }

        public DataConnection DataConnection => this;

        public ITable<Customer> Customer => this.GetTable<Customer>();

        public ITable<MeterReadings> MeterReadings => this.GetTable<MeterReadings>();
    }
}