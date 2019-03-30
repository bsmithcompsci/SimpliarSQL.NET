﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KypterUnIAPI.Core.Database
{
    public class MySQL
    {
        private static string connectionString = "";

        /// <summary>
        /// Will connect to the database, either by creating a new one or by purely connecting to an existing one.
        /// </summary>
        /// <returns></returns>
        public static async Task Connect(string source, string username, string password, string database = "", bool createDatabase = false, bool debug = false)
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
        }

        public static int Execute(string query, List<MySqlParameter> parameters = null, bool debug = false)
        {
            return new Execute(MySQL.connectionString).Execute(query, parameters, debug);
        }

        public static async Task<int> ExecuteAsync(string query, Action<int> callback, List<MySqlParameter> parameters = null, bool debug = false)
        {
            return await new Execute(MySQL.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        public static List<Dictionary<string, object>> FetchAll(string query, List<MySqlParameter> parameters = null, bool debug = false)
        {
            return new FetchAll(MySQL.connectionString).Execute(query, parameters, debug);
        }

        public static async Task<List<Dictionary<string, object>>> FetchAllAsync(string query, Action<List<Dictionary<string, object>>> callback, List<MySqlParameter> parameters = null, bool debug = false)
        {
            return await new FetchAll(MySQL.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        public static object FetchScalar(string query, List<MySqlParameter> parameters = null, bool debug = false)
        {
            return new FetchScalar(MySQL.connectionString).Execute(query, parameters, debug);
        }

        public static async Task<object> FetchScalarAsync(string query, Action<object> callback, List<MySqlParameter> parameters = null, bool debug = false)
        {
            return await new FetchScalar(MySQL.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        public static object Insert(string query, List<MySqlParameter> parameters = null, bool debug = false)
        {
            return new Insert(MySQL.connectionString).Execute(query, parameters, debug);
        }

        public static async Task<object> InsertAsync(string query, Action<object> callback, List<MySqlParameter> parameters = null, bool debug = false)
        {
            return await new Insert(MySQL.connectionString).ExecuteAsync(query, callback, parameters, debug);
        }

        #region API
        public static int CreateTable(string name, List<string> parameters, bool debug = false)
        {
            return Execute($"CREATE TABLE IF NOT EXISTS `@name` (?)", new List<MySqlParameter> { new MySqlParameter("@name", name), new MySqlParameter("?", string.Join(",", parameters)) }, debug);
        }

        public static async Task<int> CreateTableAsync(string name, Action<int> callback, List<MySqlParameter> parameters, bool debug = false)
        {
            return await ExecuteAsync($"CREATE TABLE IF NOT EXISTS `@name` (?)", callback, new List<MySqlParameter> { new MySqlParameter("@name", name), new MySqlParameter("?", string.Join(",", parameters)) }, debug);
        }

        public static int CreateDatabase(string name, bool debug = false)
        {
            if (!DatabaseExists(name, debug))
                return Execute($"CREATE DATABASE `@name`;", new List<MySqlParameter> { new MySqlParameter("@name", name) }, debug);
            else
                return -1;
        }

        public static async Task<object> CreateDatabaseAsync(string name, Action<int> callback, bool debug = false)
        {
            return await DatabaseExistsAsync(name, new Action<bool>(async (x) => {
                if (!x)
                    callback(await ExecuteAsync($"CREATE DATABASE `@name`;", null, new List<MySqlParameter> { new MySqlParameter("@name", name) }, debug));
                else
                    callback(-1);
            }), debug: debug);
        }

        public static List<Dictionary<string, object>> GetAllDatabases(bool debug = false)
        {
            return FetchAll($"SHOW DATABASES;", new List<MySqlParameter> { }, debug);
        }

        public static async Task<List<Dictionary<string, object>>> GetAllDatabasesAsync(Action<List<Dictionary<string, object>>> callback, bool debug = false)
        {
            return await FetchAllAsync($"SHOW DATABASES;", callback, new List<MySqlParameter> { }, debug);
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
            return FetchAll($"SELECT table_name FROM information_schema.tables WHERE table_schema=@database;", new List<MySqlParameter> { new MySqlParameter("@database", database) }, debug);
        }

        public static async Task<List<Dictionary<string, object>>> GetAllTablesAsync(string database, Action<List<Dictionary<string, object>>> callback, bool debug = false)
        {
            return await FetchAllAsync($"SELECT table_name FROM information_schema.tables WHERE table_schema=@database;", callback, new List<MySqlParameter> { new MySqlParameter("@database", database) }, debug);
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