using System.Data.Common;

namespace Dangl.OpenCDE.Data
{
    public interface IDapperSqlConnectionProvider
    {
        DbConnection GetSqlConnection();

        (DbConnection connection, DbTransaction transaction) GetEfCoreConnectionAndTransaction(CdeDbContext context);
    }
}
