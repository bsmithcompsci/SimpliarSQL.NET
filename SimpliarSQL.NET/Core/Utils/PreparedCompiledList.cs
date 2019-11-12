using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpliarSQL.NET.Core.Utils
{
    internal class PreparedCompiledList : IEnumerator<PreparedStatement>
    {
        private List<PreparedStatement> preparedPiars;

        private int position = -1;

        public PreparedCompiledList(List<PreparedStatement> preparedPiars)
        {
            this.preparedPiars = preparedPiars;
        }

        public PreparedStatement Current
        {
            get
            {
                try
                {
                    return preparedPiars.ToArray()[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose()
        {
            preparedPiars = new List<PreparedStatement>();
        }

        public bool MoveNext()
        {
            position++;
            return (position < preparedPiars.Count);
        }

        public void Reset()
        {
            position = -1;
        }
    }
}