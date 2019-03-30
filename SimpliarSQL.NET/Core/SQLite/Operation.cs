using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core.Database
{
    public abstract class Operation<TResult>
    {
        private string connectionString;

        public Operation(string connectionString) => this.connectionString = connectionString;

        public TResult Execute(string query, List<SQLiteParameter> parameters = null, bool debug = false)
        {
            TResult result = default(TResult);

            Stopwatch stopwatch = new Stopwatch();

            try
            {
                if (debug) stopwatch.Start();

                using (var con = new SQLiteConnection(connectionString))
                {
                    con.Open();

                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        if (parameters != null) cmd.Parameters.AddRange(parameters.ToArray());
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

                if (!(firstException is SQLiteException))
                    throw;

                Console.WriteLine(string.Format("[Query:SQL_ERROR] => [{0}] [{1}ms] {2} : {3}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters), aggExc.Message));
            }
            catch (SQLiteException sqlExc)
            {
                Console.WriteLine(string.Format("[Query:SQL_ERROR] => [{0}] [{1}ms] {2} : {3}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters), sqlExc.Message));
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("[Critical:Query:SQL_ERROR] => [{0}] [{1}ms] {2} : [{3}]{4}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters), e.GetType().Name, e.StackTrace));
            }

            return result;
        }
        public async Task<TResult> ExecuteAsync(string query, Action<TResult> asyncCallback, List<SQLiteParameter> parameters = null, bool debug = false)
        {
            TResult result = default(TResult);

            Stopwatch stopwatch = new Stopwatch();

            try
            {
                if (debug) stopwatch.Start();

                using (var con = new SQLiteConnection(connectionString))
                {
                    con.Open();

                    using (var cmd = new SQLiteCommand(query, con))
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

                if (!(firstException is SQLiteException))
                    throw;

                Console.WriteLine(string.Format("[Query:SQL_ERROR] => [{0}] [{1}ms] {2} : {3}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters), aggExc.Message));
            }
            catch (SQLiteException sqlExc)
            {
                Console.WriteLine(string.Format("[Query:SQL_ERROR] => [{0}] [{1}ms] {2} : {3}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters), sqlExc.Message));
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("[Critical:Query:SQL_ERROR] => [{0}] [{1}ms] {2} : [{3}]{4}", this.GetType().Name, stopwatch.ElapsedMilliseconds, QueryToString(query, parameters), e.GetType().Name, e.StackTrace));
            }

            return result;
        }

        abstract protected TResult Reader(SQLiteCommand cmd);
        abstract protected Task<TResult> ReaderAsync(SQLiteCommand cmd);

        private string QueryToString(string query, List<SQLiteParameter> parameters)
        {
            //return query + " {" + string.Join(";", parameters.Select(x => x.ParameterName + "=" + x.Value).ToArray() + "}");
            string result = query;



            return result;
        }
    }
}
