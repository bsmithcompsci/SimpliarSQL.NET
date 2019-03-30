﻿using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace KypterUnIAPI.Core.Database
{
    public class Execute : Operation<int>
    {
        public Execute(string connectionString) : base(connectionString) { }

        protected override int Reader(MySqlCommand cmd)
        {
            return cmd.ExecuteNonQuery();
        }

        protected override Task<int> ReaderAsync(MySqlCommand cmd)
        {
            return cmd.ExecuteNonQueryAsync();
        }
    }
}