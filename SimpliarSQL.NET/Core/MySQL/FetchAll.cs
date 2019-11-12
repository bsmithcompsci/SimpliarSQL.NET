using MySql.Data.MySqlClient;
using SimpliarSQL.NET.Core.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SimpliarSQL.NET.Core.MySQL
{
    public class FetchAll : Operation<SQLFetched>
    {
        public FetchAll(string connectionString) : base(connectionString) { }

        protected override SQLFetched Reader(MySqlCommand cmd)
        {
            var results = new List<Dictionary<string, object>>();

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
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
            return new SQLFetched(results);
        }

        protected override async Task<SQLFetched> ReaderAsync(MySqlCommand cmd)
        {
            var results = new List<Dictionary<string, object>>();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows)
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
            return new SQLFetched(results);
        }
    }
}
