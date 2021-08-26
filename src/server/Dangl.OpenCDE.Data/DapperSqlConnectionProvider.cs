using Dapper.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Dangl.OpenCDE.Data
{
    public class DapperSqlConnectionProvider : IDapperSqlConnectionProvider
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public DapperSqlConnectionProvider(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public (DbConnection connection, DbTransaction transaction) GetEfCoreConnectionAndTransaction(CdeDbContext context)
        {
            var contextConnection = context.Database.GetDbConnection();
            var contextTransaction = (context.Database.CurrentTransaction
                    as Microsoft.EntityFrameworkCore.Infrastructure.IInfrastructure<DbTransaction>)
                    .Instance;

            return (contextConnection, contextTransaction);
        }

        public DbConnection GetSqlConnection()
        {
            return _dbConnectionFactory.CreateConnection();
        }
    }
}
