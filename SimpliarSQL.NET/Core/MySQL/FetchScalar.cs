using MySql.Data.MySqlClient;
using System.Threading.Tasks;


namespace SimpliarSQL.Core.MySQL
{
    public class FetchScalar : Operation<object>
    {
        public FetchScalar(string connectionString) : base(connectionString) { }

        protected override object Reader(MySqlCommand cmd)
        {
            return cmd.ExecuteScalar();
        }

        protected override async Task<object> ReaderAsync(MySqlCommand cmd)
        {
            return await cmd.ExecuteScalarAsync();
        }
    }
}
