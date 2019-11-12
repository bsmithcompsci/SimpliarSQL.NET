using System.Collections.Generic;

namespace SimpliarSQL.NET.Core.Utils
{
    public class SessionData
    {
        private static Dictionary<string, object> sessionData = new Dictionary<string, object>();

        public static Dictionary<string, object> GetData()
        {
            return sessionData;
        }

        public static object GetData(string key)
        {
            try
            {
                sessionData.TryGetValue(key, out object val);
                return val;
            }
            catch
            {
                return null;
            }
        }

        public static void Add(string key, object value)
        {
            sessionData.Add(key, value);
        }

        public static void Remove(string key)
        {
            sessionData.Remove(key);
        }

        public static void Modify(string key, object value)
        {
            sessionData[key] = value;
        }
    }
}
