using System.Collections;
using System.Collections.Generic;

namespace SimpliarSQL.NET.Core.Utils
{
    public class PreparedList : IEnumerable<PreparedStatement>
    {
        private List<PreparedStatement> preparedStatements = new List<PreparedStatement>();

        public void Add(string key, object value)
        {
            preparedStatements.Add(new PreparedStatement(key, value));
        }
        public void Add(PreparedStatement statement)
        {
            preparedStatements.Add(statement);
        }

        public void Remove(string key)
        {
            PreparedStatement statement = FindPrepared(key);
            if (statement != null)
            {
                preparedStatements.Remove(statement);
            }
        }

        public PreparedStatement FindPrepared(string key)
        {
            foreach (PreparedStatement stmt in preparedStatements)
            {
                if (stmt.GetKey().Equals(key))
                {
                    return stmt;
                }
            }
            return null;
        }

        public object[] ToArray()
        {
            List<object> objs = new List<object>();
            foreach (PreparedStatement statement in preparedStatements)
            {
                objs.Add(statement.GetValue());
            }

            return objs.ToArray();
        }

        public IEnumerator<PreparedStatement> GetEnumerator()
        {
            return new PreparedCompiledList(preparedStatements);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) GetEnumerator();
        }
    }
}
