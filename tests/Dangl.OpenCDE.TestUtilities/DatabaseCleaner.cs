using Dangl.OpenCDE.Data;
using Dapper;
using System.Threading.Tasks;
using System.Transactions;

namespace Dangl.OpenCDE.TestUtilities
{
    public class DatabaseCleaner
    {
        private readonly IDapperSqlConnectionProvider _dapperSqlConnectionProvider;

        public DatabaseCleaner(IDapperSqlConnectionProvider dapperSqlConnectionProvider)
        {
            _dapperSqlConnectionProvider = dapperSqlConnectionProvider;
        }

        public async Task DropAllTablesInDatabaseAsync()
        {
            await DropAllTablesInDatabaseAsyncInternal();
        }

        private async Task DropAllTablesInDatabaseAsyncInternal()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await using (var connection = _dapperSqlConnectionProvider.GetSqlConnection())
            {
                await connection.OpenAsync();

                // Essentially taken from here: https://stackoverflow.com/a/43128914/4190785
                // This command first removes all foreign keys, then deletes all tables
                var dropAllCommand = @"DECLARE @sql NVARCHAR(2000)

WHILE(EXISTS(SELECT 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE='FOREIGN KEY' AND TABLE_SCHEMA = 'dbo'))
BEGIN
    SELECT TOP 1 @sql=('ALTER TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + '] DROP CONSTRAINT [' + CONSTRAINT_NAME + ']')
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'
    AND TABLE_SCHEMA = 'dbo'
    EXEC(@sql)
    PRINT @sql
END

WHILE(EXISTS(SELECT * from INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo'))
BEGIN
    SELECT TOP 1 @sql=('DROP TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + ']')
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'dbo'
    EXEC(@sql)
    PRINT @sql
END";

                await connection.ExecuteAsync(dropAllCommand);
                transactionScope.Complete();
            }
        }
    }
}
