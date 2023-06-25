using System.Collections;
using System.Linq.Expressions;
using LinqToDB;
using LinqToDB.Linq;

namespace MeterReadingApiTests.Mocks
{
    public class MockDatabaseTable<T> : ITable<T>
	{
        private readonly IList<T> tableDataList;

        public MockDatabaseTable(IList<T> tableDataList)
        {
            this.tableDataList = tableDataList;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.tableDataList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.tableDataList.GetEnumerator();
        }

        public Expression Expression
        {
            get => this.tableDataList.AsQueryable().Expression;
            set => throw new NotImplementedException();
        }

        public Type ElementType => this.tableDataList.AsQueryable().ElementType;

        public IQueryProvider Provider => this.tableDataList.AsQueryable().Provider;

        Expression IQueryable.Expression => this.tableDataList.AsQueryable().Expression;

        public string DatabaseName => throw new NotImplementedException();

        public string SchemaName => throw new NotImplementedException();

        public string TableName => throw new NotImplementedException();

        public string SqlText => throw new NotImplementedException();

        public IDataContext DataContext => throw new NotImplementedException();

        Expression IExpressionQuery.Expression => throw new NotImplementedException();

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object? Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public LinqToDB.Async.IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            throw new NotImplementedException();
        }        

        public string GetTableName()
        {
            throw new NotImplementedException();
        }        
    }
}

