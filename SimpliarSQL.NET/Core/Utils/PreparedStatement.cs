namespace SimpliarSQL.NET.Core.Utils
{
    public class PreparedStatement
    {
        private string key;
        private object value;

        public PreparedStatement(string key, object value)
        {
            this.key = key;
            this.value = value;
        }

        public string GetKey()
        {
            return this.key;
        }

        public object GetValue()
        {
            return this.value;
        }
    }
}
