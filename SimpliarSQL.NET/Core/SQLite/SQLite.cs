using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace SimpliarSQL.Core.SQLite
{
    public class SQLite
    {
        private static string connectionString = "";

        /// <summary>
        /// Will connect to the database, either by creating a new one or by purely connecting to an existing one.
        /// </summary>
        /// <returns></returns>
        public static Task Verify(string filename, string path = "")
        {
            string reference = Path.Combine(Path.GetFullPath("./"), path) + filename + ".sqlite";
            if (!File.Exists(reference))
                SQLiteConnection.CreateFile(reference);

            SQLite.connectionString = "Data Source=" + reference;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes SQL Command to SQLite's File...
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static int Execute(string query, List<SQLiteParameter> parameters = null, bool debug = false)
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
        public static async Task<int> ExecuteAsync(string query, Action<int> callback, List<SQLiteParameter> parameters = null, bool debug = false)
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
        public static List<Dictionary<string, object>> FetchAll(string query, List<SQLiteParameter> parameters = null, bool debug = false)
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
        public static async Task<List<Dictionary<string, object>>> FetchAllAsync(string query, Action<List<Dictionary<string, object>>> callback, List<SQLiteParameter> parameters = null, bool debug = false)
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
        public static object FetchScalar(string query, List<SQLiteParameter> parameters = null, bool debug = false)
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
        public static async Task<object> FetchScalarAsync(string query, Action<object> callback, List<SQLiteParameter> parameters = null, bool debug = false)
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
        public static object Insert(string query, List<SQLiteParameter> parameters = null, bool debug = false)
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
        public static async Task<object> InsertAsync(string query, Action<object> callback, List<SQLiteParameter> parameters = null, bool debug = false)
        {
            return await new Insert(SQLite.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        #region API
        public static int CreateTable(string name, List<string> parameters, bool debug = false)
        {
            return Execute($"CREATE TABLE IF NOT EXISTS `@name` (?)", new List<SQLiteParameter> { new SQLiteParameter("@name", name), new SQLiteParameter("?", string.Join(",", parameters)) }, debug);
        }

        public static async Task<int> CreateTableAsync(string name, Action<int> callback, List<SQLiteParameter> parameters, bool debug = false)
        {
            return await ExecuteAsync($"CREATE TABLE IF NOT EXISTS `@name` (?)", callback, new List<SQLiteParameter> { new SQLiteParameter("@name", name), new SQLiteParameter("?", string.Join(",", parameters)) }, debug);
        }

        public static int CreateDatabase(string name, bool debug = false)
        {
            if (!DatabaseExists(name, debug))
                return Execute($"CREATE DATABASE `@name`;", new List<SQLiteParameter> { new SQLiteParameter("@name", name) }, debug);
            else
                return -1;
        }

        public static async Task<object> CreateDatabaseAsync(string name, Action<int> callback, bool debug = false)
        {
            return await DatabaseExistsAsync(name, new Action<bool>(async (x) => {
                if (!x)
                    callback(await ExecuteAsync($"CREATE DATABASE `@name`;", null, new List<SQLiteParameter> { new SQLiteParameter("@name", name) }, debug));
                else
                    callback(-1);
            }), debug: debug);
        }

        public static List<Dictionary<string, object>> GetAllDatabases(bool debug = false)
        {
            return FetchAll($"SHOW DATABASES;", new List<SQLiteParameter> { }, debug);
        }

        public static async Task<List<Dictionary<string, object>>> GetAllDatabasesAsync(Action<List<Dictionary<string, object>>> callback, bool debug = false)
        {
            return await FetchAllAsync($"SHOW DATABASES;", callback, new List<SQLiteParameter> { }, debug);
        }

        public static bool DatabaseExists(string database, bool debug = false)
        {
            List<Dictionary<string, object>> databases = GetAllDatabases(debug);

            foreach (var list in databases)
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
            return await GetAllDatabasesAsync(new Action<List<Dictionary<string, object>>>((x) => {
                bool found = false;
                foreach (var list in x)
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

        public static List<Dictionary<string, object>> GetAllTables(string database, bool debug = false)
        {
            return FetchAll($"SELECT table_name FROM information_schema.tables WHERE table_schema=@database;", new List<SQLiteParameter> { new SQLiteParameter("@database", database) }, debug);
        }

        public static async Task<List<Dictionary<string, object>>> GetAllTablesAsync(string database, Action<List<Dictionary<string, object>>> callback, bool debug = false)
        {
            return await FetchAllAsync($"SELECT table_name FROM information_schema.tables WHERE table_schema=@database;", callback, new List<SQLiteParameter> { new SQLiteParameter("@database", database) }, debug);
        }

        public static bool TablesExists(string database, string table, bool debug = false)
        {
            List<Dictionary<string, object>> databases = GetAllTables(database, debug);

            foreach (var list in databases)
            {
                foreach (var pair in list)
                {
                    if (pair.Value.ToString().ToLower() == table.ToLower())
                        return true;
                }
            }

            return false;
        }

        public static async Task<object> TablesExistsAsync(string database, string table, Action<bool> callback, bool debug = false)
        {
            return await GetAllTablesAsync(database, new Action<List<Dictionary<string, object>>>((x) => {
                foreach (var list in x)
                {
                    foreach (var pair in list)
                    {
                        if (pair.Value.ToString().ToLower() == table.ToLower())
                            callback(true);
                    }
                }
                callback(false);
            }), debug);
        }
        #endregion
    }
}
