using SimpliarSQL.NET.Core.Utils;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace SimpliarSQL.NET.Core.SQLite
{
    public class FetchAll : Operation<SQLReturnFetched>
    {
        public FetchAll(string connectionString) : base(connectionString) { }

        protected override SQLReturnFetched Reader(SQLiteCommand cmd)
        {
            var results = new List<Dictionary<string, object>>();

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        var line = new Dictionary<string, object>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.IsDBNull(i))
                                line.Add(reader.GetName(i), null);
                            else
                                line.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        results.Add(line);
                    }
                }
            }
            return new SQLReturnFetched(results);
        }

        protected override async Task<SQLReturnFetched> ReaderAsync(SQLiteCommand cmd)
        {
            var results = new List<Dictionary<string, object>>();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        var line = new Dictionary<string, object>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.IsDBNull(i))
                                line.Add(reader.GetName(i), null);
                            else
                                line.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        results.Add(line);
                    }
                }
            }
            return new SQLReturnFetched(results);
        }
    }
}
