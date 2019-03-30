using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core.Database
{
    public class FetchScalar : Operation<object>
    {
        public FetchScalar(string connectionString) : base(connectionString) { }

        protected override object Reader(SQLiteCommand cmd)
        {
            return cmd.ExecuteScalar();
        }

        protected override async Task<object> ReaderAsync(SQLiteCommand cmd)
        {
            return await cmd.ExecuteScalarAsync();
        }
    }
}
