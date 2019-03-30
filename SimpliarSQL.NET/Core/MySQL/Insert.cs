using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace KypterUnIAPI.Core.Database
{
    public class Insert : Operation<object>
    {
        public Insert(string connectionString) : base(connectionString) { }

        protected override object Reader(MySqlCommand cmd)
        {
            cmd.ExecuteNonQuery();

            using (var lastInsertCommand = cmd.Connection.CreateCommand())
            {
                lastInsertCommand.CommandText = "SELECT LAST_INSERT_ID()";
                return lastInsertCommand.ExecuteScalar();
            }
        }

        protected override async Task<object> ReaderAsync(MySqlCommand cmd)
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
