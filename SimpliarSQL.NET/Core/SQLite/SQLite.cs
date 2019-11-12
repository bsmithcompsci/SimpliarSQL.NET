using SimpliarSQL.NET.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace SimpliarSQL.NET.Core.SQLite
{
    public class SQLite
    {
        private static string connectionString = "";

        /// <summary>
        /// Will connect to the database, either by creating a new one or by purely connecting to an existing one.
        /// </summary>
        /// <returns></returns>
        public static string Verify(string filename, string path = "")
        {
            string reference = Path.Combine(Path.GetFullPath("./"), path) + filename + ".sqlite";
            if (!File.Exists(reference))
                SQLiteConnection.CreateFile(reference);

            SQLite.connectionString = "Data Source=" + reference;

            return connectionString;
        }

        /// <summary>
        /// Grabs the local connection string...
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            return connectionString;
        }

        /// <summary>
        /// Executes SQL Command to SQLite's File...
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static int Execute(string query, PreparedList parameters = null, bool debug = false)
        {
            return new Execute(SQLite.connectionString).Execute(query, parameters, debug);
        }

        /// <summary>
        /// Executes SQL Command to SQLite's File...
        /// </summary>
        /// <param name="query"></param>
        /// <param name="callback"></param>
        /// <param name="parameters"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteAsync(string query, Action<int> callback, PreparedList parameters = null, bool debug = false)
        {
            return await new Execute(SQLite.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        /// <summary>
        /// Fetches all the result data to the function
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static SQLFetched FetchAll(string query, PreparedList parameters = null, bool debug = false)
        {
            return new FetchAll(SQLite.connectionString).Execute(query, parameters, debug);
        }

        /// <summary>
        /// Fetches all the result data to the function
        /// </summary>
        /// <param name="query"></param>
        /// <param name="callback"></param>
        /// <param name="parameters"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static async Task<SQLFetched> FetchAllAsync(string query, Action<SQLFetched> callback, PreparedList parameters = null, bool debug = false)
        {
            return await new FetchAll(SQLite.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        /// <summary>
        /// Fetches the amount of result data to the function.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static object FetchScalar(string query, PreparedList parameters = null, bool debug = false)
        {
            return new FetchScalar(SQLite.connectionString).Execute(query, parameters, debug);
        }

        /// <summary>
        /// Fetches the amount of result data to the function.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static async Task<object> FetchScalarAsync(string query, Action<object> callback, PreparedList parameters = null, bool debug = false)
        {
            return await new FetchScalar(SQLite.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        /// <summary>
        /// Inserts Data into the database's rows
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static object Insert(string query, PreparedList parameters = null, bool debug = false)
        {
            return new Insert(SQLite.connectionString).Execute(query, parameters, debug);
        }

        /// <summary>
        /// Inserts Data into the database's rows
        /// </summary>
        /// <param name="query"></param>
        /// <param name="callback"></param>
        /// <param name="parameters"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static async Task<object> InsertAsync(string query, Action<object> callback, PreparedList parameters = null, bool debug = false)
        {
            return await new Insert(SQLite.connectionString).ExecuteAsync(query, callback, parameters, debug);
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

        public static SQLFetched GetAllDatabases(bool debug = false)
        {
            return FetchAll($"SHOW DATABASES;", new PreparedList { }, debug);
        }

        public static async Task<SQLFetched> GetAllDatabasesAsync(Action<SQLFetched> callback, bool debug = false)
        {
            return await FetchAllAsync($"SHOW DATABASES;", callback, new PreparedList { }, debug);
        }

        public static bool DatabaseExists(string database, bool debug = false)
        {
            SQLFetched databases = GetAllDatabases(debug);

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
            return await GetAllDatabasesAsync(new Action<SQLFetched>((x) => {
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

        public static SQLFetched GetAllTables(string database, bool debug = false)
        {
            return FetchAll($"SELECT table_name FROM information_schema.tables WHERE table_schema=@database;", new PreparedList { new PreparedStatement("@database", database) }, debug);
        }

        public static async Task<SQLFetched> GetAllTablesAsync(string database, Action<SQLFetched> callback, bool debug = false)
        {
            return await FetchAllAsync($"SELECT table_name FROM information_schema.tables WHERE table_schema=@database;", callback, new PreparedList { new PreparedStatement("@database", database) }, debug);
        }

        public static bool TablesExists(string database, string table, bool debug = false)
        {
            SQLFetched databases = GetAllTables(database, debug);

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
                SQLFetched databases = GetAllTables(database, debug);

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
