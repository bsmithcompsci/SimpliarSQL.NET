using MySql.Data.MySqlClient;
using SimpliarSQL.NET.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpliarSQL.NET.Core.MySQL
{
    public abstract class Operation<TResult>
    {
        private string connectionString;

        public Operation(string connectionString) => this.connectionString = connectionString;

        public TResult Execute(string query, PreparedList parameters = null, bool debug = false)
        {
            TResult result = default(TResult);

            Stopwatch stopwatch = new Stopwatch();

            try
            {
                if (debug) stopwatch.Start();

                using (var con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        if (parameters != null)
                        {
                            List<MySqlParameter> _parameters = new List<MySqlParameter>();
                            foreach (PreparedStatement statement in parameters)
                            {
                                _parameters.Add(new MySqlParameter(statement.GetKey(), statement.GetValue()));
                            }
                            cmd.Parameters.AddRange(_parameters.ToArray());
                        }
                        result = Reader(cmd);

                        if (debug)
                        {
                            stopwatch.Stop();
                            Console.WriteLine(string.Format("[{0}] [{1}ms] {2}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters)));
                        }
                    }
                }
            }
            catch (AggregateException aggExc)
            {
                var firstException = aggExc.InnerExceptions.First();

                if (!(firstException is MySqlException))
                    throw;

                Console.WriteLine(string.Format("[Query:SQL_ERROR] => [{0}] [{1}ms] {2} : {3}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters), aggExc.Message));
            }
            catch (MySqlException sqlExc)
            {
                Console.WriteLine(string.Format("[Query:SQL_ERROR] => [{0}] [{1}ms] {2} : {3}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters), sqlExc.Message));
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("[Critical:Query:SQL_ERROR] => [{0}] [{1}ms] {2} : [{3}]{4}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters), e.GetType().Name, e.StackTrace));
            }

            return result;
        }
        public async Task<TResult> ExecuteAsync(string query, Action<TResult> asyncCallback, PreparedList parameters = null, bool debug = false)
        {
            TResult result = default(TResult);

            Stopwatch stopwatch = new Stopwatch();

            try
            {
                if (debug) stopwatch.Start();

                using (var con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        if (parameters != null) cmd.Parameters.AddRange(parameters.ToArray());
                        result = await ReaderAsync(cmd);

                        if (debug)
                        {
                            stopwatch.Stop();
                            Console.WriteLine(string.Format("[{0}] [{1}ms] {2}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters)));
                        }

                        asyncCallback?.Invoke(result);
                    }
                }
            }
            catch (AggregateException aggExc)
            {
                var firstException = aggExc.InnerExceptions.First();

                if (!(firstException is MySqlException))
                    throw;

                Console.WriteLine(string.Format("[QueryAsync:SQL_ERROR] => [{0}] [{1}ms] {2} : {3}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters), aggExc.Message));
            }
            catch (MySqlException sqlExc)
            {
                Console.WriteLine(string.Format("[QueryAsync:SQL_ERROR] => [{0}] [{1}ms] {2} : {3}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters), sqlExc.Message));
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("[Critical:QueryAsync:SQL_ERROR] => [{0}] [{1}ms] {2} : [{3}]{4}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters), e.GetType().Name, e.StackTrace));
            }

            return result;
        }

        abstract protected TResult Reader(MySqlCommand cmd);
        abstract protected Task<TResult> ReaderAsync(MySqlCommand cmd);

        private string QueryToString(string query, PreparedList parameters)
        {
            //return query + " {" + string.Join(";", parameters.Select(x => x.ParameterName + "=" + x.Value).ToArray() + "}");
            string result = query;



            return result;
        }
    }
}
