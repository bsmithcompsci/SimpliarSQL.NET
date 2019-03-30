using System.Data.SQLite;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core.Database
{
    public class Insert : Operation<object>
    {
        public Insert(string connectionString) : base(connectionString) { }

        protected override object Reader(SQLiteCommand cmd)
        {
            cmd.ExecuteNonQuery();

            using (var lastInsertCommand = cmd.Connection.CreateCommand())
            {
                lastInsertCommand.CommandText = "SELECT LAST_INSERT_ID()";
                return lastInsertCommand.ExecuteScalar();
            }
        }

        protected override async Task<object> ReaderAsync(SQLiteCommand cmd)
        {
            await cmd.ExecuteNonQueryAsync();

            using (var lastInsertCommand = cmd.Connection.CreateCommand())
            {
                lastInsertCommand.CommandText = "SELECT LAST_INSERT_ID()";
                return await lastInsertCommand.ExecuteScalarAsync();
            }
        }
    }
}
