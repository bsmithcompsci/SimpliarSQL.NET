﻿using System.Collections.Generic;

namespace SimpliarSQL.NET.Core.Utils
{
    public class SQLReturnFetched
    {
        private List<Dictionary<string, object>> rows;

        public SQLReturnFetched(List<Dictionary<string, object>> rows)
        {
            this.rows = rows;
        }

        public List<Dictionary<string, object>> FindAllByKey(params string[] keys)
        {
            List<Dictionary<string, object>> accepted_rows = new List<Dictionary<string, object>>();
            foreach (Dictionary<string, object> row in rows)
            {
                foreach(string key in keys)
                {
                    if (row.ContainsKey(key))
                    {
                        rows.Add(row);
                        break;
                    }
                }
            }
            return accepted_rows;
        }

        public Dictionary<string, object> FindOneByKey(string key)
        {
            foreach (Dictionary<string, object> row in rows)
            {
                if (row.ContainsKey(key))
                {
                    return row;
                }
            }
            return null;
        }

        public int GetCount()
        {
            return rows.Count;
        }

        public List<Dictionary<string, object>> GetRows()
        {
            return rows;
        }
    }
}
