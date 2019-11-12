using SimpliarSQL.NET.Core.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpliarSQL.NET.Core
{
    public interface ISQLModule { }

    public enum ESQLTypes
    {
        SQLite,
        MySQL
    }

    public class SQLCore
    {
        protected ESQLTypes activeType;
        protected string connection_string;
        
        /// <summary>
        /// Initializes the SQL Core to the proper SQL Library.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="database"></param>
        public void Initialize(ESQLTypes type, string source = "", string username = "", string password = "", string database = "")
        {
            this.activeType = type;

            switch (activeType)
            {
                case ESQLTypes.MySQL:
                    connection_string = MySQL.MySQL.Connect(source, username, password, database, true);
                    break;
                case ESQLTypes.SQLite:
                    connection_string = SQLite.SQLite.Verify(database);
                    break;
            }
        }

        /// <summary>
        /// Initializes the SQLite.SQLite in a easier function.
        /// </summary>
        /// <param name="database"></param>
        public void Initialize(string database)
        {
            Initialize(ESQLTypes.SQLite, database: database);
        }

        /// <summary>
        /// Asyncingly Initializes the SQL Core to the proper SQL Library.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public async Task InitializeAsync(ESQLTypes type, string source = "", string username = "", string password = "", string database = "")
        {
            this.activeType = type;

            switch (activeType)
            {
                case ESQLTypes.MySQL:
                    connection_string = await MySQL.MySQL.ConnectAsync(source, username, password, database, true);
                    break;
                case ESQLTypes.SQLite:
                    connection_string = SQLite.SQLite.Verify(database);
                    break;
            }
        }
        
        /// <summary>
        /// Creates a new Table out of a Class Structure...
        /// </summary>
        /// <param name="structure"></param>
        public void CreateSQLStructure(ISQLModule structure)
        {
            string tableName = structure.GetType().Name;
            List<string> variables = new List<string>();
            foreach(var prop in structure.GetType().GetProperties())
            {
                Console.WriteLine("{0}={1} [{2}]", prop.Name, prop.GetValue(structure, null), prop.GetValue(structure, null).GetType().ToString());

                switch(prop.GetValue(structure, null).GetType().ToString())
                {
                    case "string":
                        break;
                    case "int":
                        break;
                    case "boolean":
                        break;
                }
            }

            Console.WriteLine("CREATE TABLE IF NOT EXISTS `{0}` ({1})", tableName, String.Join(",", variables));
        }

        public int CreateTable(string name, List<string> parameters, bool debug = false)
        {
            switch(this.activeType)
            {
                case ESQLTypes.MySQL:
                    return MySQL.MySQL.CreateTable(name, parameters, debug);
                case ESQLTypes.SQLite:
                    return SQLite.SQLite.CreateTable(name, parameters, debug);
            }
            return -1;
        }

        public async Task<int> CreateTableAsync(string name, Action<int> callback, List<string> parameters, bool debug = false)
        {
            switch (this.activeType)
            {
                case ESQLTypes.MySQL:
                    return await MySQL.MySQL.CreateTableAsync(name, callback, parameters, debug);
                case ESQLTypes.SQLite:
                    return await SQLite.SQLite.CreateTableAsync(name, callback, parameters, debug);
            }
            return -1;
        }

        public int CreateDatabase(string name, bool debug = false)
        {
            switch (this.activeType)
            {
                case ESQLTypes.MySQL:
                    return MySQL.MySQL.CreateDatabase(name, debug);
                case ESQLTypes.SQLite:
                    return SQLite.SQLite.CreateDatabase(name, debug);
            }
            return -1;
        }

        public async Task<object> CreateDatabaseAsync(string name, Action<int> callback, bool debug = false)
        {
            switch (this.activeType)
            {
                case ESQLTypes.MySQL:
                    return await MySQL.MySQL.CreateDatabaseAsync(name, callback, debug);
                case ESQLTypes.SQLite:
                    return await SQLite.SQLite.CreateDatabaseAsync(name, callback, debug);
            }
            return -1;
        }

        public SQLFetched GetAllDatabases(bool debug = false)
        {
            switch (this.activeType)
            {
                case ESQLTypes.MySQL:
                    return MySQL.MySQL.GetAllDatabases(debug);
                case ESQLTypes.SQLite:
                    return SQLite.SQLite.GetAllDatabases(debug);
            }
            return new SQLFetched(new List<Dictionary<string, object>>());
        }

        public async Task<SQLFetched> GetAllDatabasesAsync(Action<SQLFetched> callback, bool debug = false)
        {
            switch (this.activeType)
            {
                case ESQLTypes.MySQL:
                    return await MySQL.MySQL.GetAllDatabasesAsync(callback, debug);
                case ESQLTypes.SQLite:
                    return await SQLite.SQLite.GetAllDatabasesAsync(callback, debug);
            }
            return new SQLFetched(new List<Dictionary<string, object>>());
        }

        public bool DatabaseExists(string database, bool debug = false)
        {
            switch (this.activeType)
            {
                case ESQLTypes.MySQL:
                    return MySQL.MySQL.DatabaseExists(database, debug);
                case ESQLTypes.SQLite:
                    return SQLite.SQLite.DatabaseExists(database, debug);
            }
            return false;
        }

        public async Task<object> DatabaseExistsAsync(string database, Action<bool> callback, bool debug = false)
        {
            switch (this.activeType)
            {
                case ESQLTypes.MySQL:
                    return await MySQL.MySQL.DatabaseExistsAsync(database, callback, debug);
                case ESQLTypes.SQLite:
                    return await SQLite.SQLite.DatabaseExistsAsync(database, callback, debug);
            }
            return false;
        }

        public SQLFetched GetAllTables(string database, bool debug = false)
        {
            switch (this.activeType)
            {
                case ESQLTypes.MySQL:
                    return MySQL.MySQL.GetAllTables(database, debug);
                case ESQLTypes.SQLite:
                    return SQLite.SQLite.GetAllTables(database, debug);
            }
            return new SQLFetched(new List<Dictionary<string, object>>());
        }

        public async Task<SQLFetched> GetAllTablesAsync(string database, Action<SQLFetched> callback, bool debug = false)
        {
            switch (this.activeType)
            {
                case ESQLTypes.MySQL:
                    return await MySQL.MySQL.GetAllTablesAsync(database, callback, debug);
                case ESQLTypes.SQLite:
                    return await SQLite.SQLite.GetAllTablesAsync(database, callback, debug);
            }
            return new SQLFetched(new List<Dictionary<string, object>>());
        }

        public bool TablesExists(string database, string table, bool debug = false)
        {
            switch (this.activeType)
            {
                case ESQLTypes.MySQL:
                    return MySQL.MySQL.TablesExists(database, table, debug);
                case ESQLTypes.SQLite:
                    return SQLite.SQLite.TablesExists(database, table, debug);
            }
            return false;
        }

        public async Task<bool> TablesExistsAsync(string database, string table, Action<bool> callback, bool debug = false)
        {
            switch (this.activeType)
            {
                case ESQLTypes.MySQL:
                    return await MySQL.MySQL.TablesExistsAsync(database, table, callback, debug);
                case ESQLTypes.SQLite:
                    return await SQLite.SQLite.TablesExistsAsync(database, table, callback, debug);
            }
            return false;
        }



    }
}
