using System.Data.SQLite;
using System.Threading.Tasks;

namespace SimpliarSQL.NET.Core.SQLite
{
    public class Execute : Operation<int>
    {
        public Execute(string connectionString) : base(connectionString) { }

        protected override int Reader(SQLiteCommand cmd)
        {
            return cmd.ExecuteNonQuery();
        }

        protected override Task<int> ReaderAsync(SQLiteCommand cmd)
        {
            return cmd.ExecuteNonQueryAsync();
        }
    }
}
