using MySql.Data.MySqlClient;
using SimpliarSQL.NET.Core.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SimpliarSQL.NET.Core.MySQL
{
    public class MySQL
    {
        private static string connectionString = "";

        public static string Connect(string source, string username, string password, string database = "", bool createDatabase = false, bool debug = false)
        {

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = source,
                Database = (!createDatabase ? database : ""),
                UserID = username,
                Password = password
            };

            MySQL.connectionString = builder.ToString();

            if (debug || createDatabase)
                try
                {
                    var con = new MySqlConnection
                    {
                        ConnectionString = MySQL.connectionString
                    };
                    con.Open();

                    if (createDatabase)
                        CreateDatabase(database, debug);

                    con.Close();

                    Console.WriteLine($"Connection to SQL Service was successful -> {source}:{3306} | {database}");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"Failed Connection to SQL Service -> {source}:{3306} | {database} :: " + ex.Message);
                }
            return connectionString;
        }

        /// <summary>
        /// Will connect to the database, either by creating a new one or by purely connecting to an existing one.
        /// </summary>
        /// <returns></returns>
        public static async Task<string> ConnectAsync(string source, string username, string password, string database = "", bool createDatabase = false, bool debug = false)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = source,
                Database = (!createDatabase ? database : ""),
                UserID = username,
                Password = password
            };

            MySQL.connectionString = builder.ToString();

            if (debug || createDatabase)
                try
                {
                    var con = new MySqlConnection
                    {
                        ConnectionString = MySQL.connectionString
                    };
                    await con.OpenAsync();

                    if (createDatabase)
                        await CreateDatabaseAsync(database, new Action<int>((x) => {
                            if (x > 0)
                                Console.WriteLine($"Created {database} Database!");
                        }));

                    await con.CloseAsync();

                    Console.WriteLine($"Connection to SQL Service was successful -> {source}:{3306} | {database}");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"Failed Connection to SQL Service -> {source}:{3306} | {database} :: " + ex.Message);
                }
            return connectionString;
        }

        public static int Execute(string query, PreparedList parameters = null, bool debug = false)
        {
            return new Execute(MySQL.connectionString).Execute(query, parameters, debug);
        }

        public static async Task<int> ExecuteAsync(string query, Action<int> callback, PreparedList parameters = null, bool debug = false)
        {
            return await new Execute(MySQL.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        public static SQLReturnFetched FetchAll(string query, PreparedList parameters = null, bool debug = false)
        {
            return new FetchAll(MySQL.connectionString).Execute(query, parameters, debug);
        }

        public static async Task<SQLReturnFetched> FetchAllAsync(string query, Action<SQLReturnFetched> callback, PreparedList parameters = null, bool debug = false)
        {
            return await new FetchAll(MySQL.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        public static object FetchScalar(string query, PreparedList parameters = null, bool debug = false)
        {
            return new FetchScalar(MySQL.connectionString).Execute(query, parameters, debug);
        }

        public static async Task<object> FetchScalarAsync(string query, Action<object> callback, PreparedList parameters = null, bool debug = false)
        {
            return await new FetchScalar(MySQL.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        public static object Insert(string query, PreparedList parameters = null, bool debug = false)
        {
            return new Insert(MySQL.connectionString).Execute(query, parameters, debug);
        }

        public static async Task<object> InsertAsync(string query, Action<object> callback, PreparedList parameters = null, bool debug = false)
        {
            return await new Insert(MySQL.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        #region API
        public static int CreateTable(string name, List<string> parameters, bool debug = false)
        {
            return Execute($"CREATE TABLE IF NOT EXISTS `@name` (?)", new PreparedList { new PreparedStatement("@name", name), new PreparedStatement("?", string.Join(",", parameters)) }, debug);
        }

        public static async Task<int> CreateTableAsync(string name, Action<int> callback, List<string> parameters, bool debug = false)
        {
            return await ExecuteAsync($"CREATE TABLE IF NOT EXISTS `@name` (?)", callback, new PreparedList { new PreparedStatement("@name", name), new PreparedStatement("?", string.Join(",", parameters)) }, debug);
        }

        public static int CreateDatabase(string name, bool debug = false)
        {
            if (!DatabaseExists(name, debug))
                return Execute($"CREATE DATABASE `@name`;", new PreparedList { new PreparedStatement("@name", name) }, debug);
            else
                return -1;
        }

        public static async Task<object> CreateDatabaseAsync(string name, Action<int> callback, bool debug = false)
        {
            return await DatabaseExistsAsync(name, new Action<bool>(async (x) => {
                if (!x)
                    callback(await ExecuteAsync($"CREATE DATABASE `@name`;", null, new PreparedList { new PreparedStatement("@name", name) }, debug));
                else
                    callback(-1);
            }), debug: debug);
        }

        public static SQLReturnFetched GetAllDatabases(bool debug = false)
        {
            return FetchAll($"SHOW DATABASES;", new PreparedList { }, debug);
        }

        public static async Task<SQLReturnFetched> GetAllDatabasesAsync(Action<SQLReturnFetched> callback, bool debug = false)
        {
            return await FetchAllAsync($"SHOW DATABASES;", callback, new PreparedList { }, debug);
        }

        public static bool DatabaseExists(string database, bool debug = false)
        {
            SQLReturnFetched databases = GetAllDatabases(debug);

            foreach (var list in databases.GetRows())
            {
                foreach (var pair in list)
                {
                    if (pair.Value.ToString().ToLower() == database.ToLower())
                        return true;
                }
            }

            return false;
        }

        public static async Task<object> DatabaseExistsAsync(string database, Action<bool> callback, bool debug = false)
        {
            return await GetAllDatabasesAsync(new Action<SQLReturnFetched>((x) => {
                bool found = false;
                foreach (var list in x.GetRows())
                {
                    foreach (var pair in list)
                    {
                        if (pair.Value.ToString().ToLower() == database.ToLower())
                        {
                            callback(true);
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }
                if (!found)
                    callback(false);
            }), debug);
        }

        public static SQLReturnFetched GetAllTables(string database, bool debug = false)
        {
            return FetchAll($"SELECT table_name FROM information_schema.tables WHERE table_schema=@database;", new PreparedList { new PreparedStatement("@database", database) }, debug);
        }

        public static async Task<SQLReturnFetched> GetAllTablesAsync(string database, Action<SQLReturnFetched> callback, bool debug = false)
        {
            return await FetchAllAsync($"SELECT table_name FROM information_schema.tables WHERE table_schema=@database;", callback, new PreparedList { new PreparedStatement("@database", database) }, debug);
        }

        public static bool TablesExists(string database, string table, bool debug = false)
        {
            SQLReturnFetched databases = GetAllTables(database, debug);

            foreach (var list in databases.GetRows())
            {
                foreach (var pair in list)
                {
                    if (pair.Value.ToString().ToLower() == table.ToLower())
                        return true;
                }
            }

            return false;
        }

        public static async Task<bool> TablesExistsAsync(string database, string table, Action<bool> callback, bool debug = false)
        {
            return await Task.Run(() =>
            {
                SQLReturnFetched databases = GetAllTables(database, debug);

                foreach (var list in databases.GetRows())
                {
                    foreach (var pair in list)
                    {
                        if (pair.Value.ToString().ToLower() == table.ToLower())
                        {
                            callback(true);
                            return true;
                        }
                    }
                }
                callback(false);
                return false;
            });
        }
        #endregion
    }
}
