using System;
using System.Data;
using Npgsql;

namespace Greatbone.Core
{
    public class DbContext : IDisposable, IResultSet
    {
        readonly NpgsqlConnection connection;

        readonly NpgsqlCommand command;

        readonly DbParameters parameters;

        private NpgsqlTransaction transact;

        private NpgsqlDataReader reader;

        bool disposed;


        internal DbContext(NpgsqlConnectionStringBuilder builder)
        {
            connection = new NpgsqlConnection(builder);
            command = new NpgsqlCommand();
            parameters = new DbParameters(command.Parameters);
            command.Connection = connection;
        }

        public void Begin()
        {
            if (transact == null)
            {
                transact = connection.BeginTransaction();
                command.Transaction = transact;
            }
        }

        public void Begin(IsolationLevel level)
        {
            if (transact == null)
            {
                transact = connection.BeginTransaction(level);
                command.Transaction = transact;
            }
        }

        public void Commit()
        {
            if (transact != null)
            {
                transact.Commit();
                command.Transaction = null;
                transact = null;
            }
        }

        public void Rollback()
        {
            if (transact != null)
            {
                transact.Rollback();
                command.Transaction = null;
                transact = null;
            }
        }

        public bool QueryA(string cmdtext, Action<DbParameters> ps)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
            // setup command
            command.CommandText = cmdtext;
            command.CommandType = CommandType.Text;
            parameters.Clear();
            ps?.Invoke(parameters);
            ordinal = 0;
            reader = command.ExecuteReader();
            return reader.Read();
        }

        public bool Query(string cmdtext, Action<DbParameters> ps)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
            // setup command
            command.CommandText = cmdtext;
            command.CommandType = CommandType.Text;
            parameters.Clear();
            ps?.Invoke(parameters);
            ordinal = 0;
            reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public bool NextRow()
        {
            ordinal = 0; // reset column ordinal

            if (reader == null)
            {
                return false;
            }
            return reader.Read();
        }

        public bool HasRows
        {
            get
            {
                if (reader == null)
                {
                    return false;
                }
                return reader.HasRows;
            }
        }

        public bool NextResult()
        {
            ordinal = 0; // reset column ordinal

            if (reader == null)
            {
                return false;
            }
            return reader.NextResult();
        }

        public int Execute(string cmdtext, Action<DbParameters> ps)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
            // setup command
            command.CommandText = cmdtext;
            command.CommandType = CommandType.Text;
            parameters.Clear();
            ps?.Invoke(parameters);
            ordinal = 0;
            return command.ExecuteNonQuery();
        }

        //
        // RESULTSET
        //

        // current column ordinal
        int ordinal;


        public bool Got(ref bool v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetBoolean(ord);
                return true;
            }
            return false;
        }

        public bool Got(ref short v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt16(ord);
                return true;
            }
            return false;
        }

        public bool Got(ref int v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt32(ord);
                return true;
            }
            return false;
        }

        public bool Got(ref long v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt64(ord);
                return true;
            }
            return false;
        }

        public bool Got(ref decimal v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetDecimal(ord);
                return true;
            }
            return false;
        }

        public bool Got(ref DateTime v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetDateTime(ord);
                return true;
            }
            return false;
        }

        public bool Got(ref char[] v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<char[]>(ord);
                return true;
            }
            return false;
        }

        public bool Got(ref string v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetString(ord);
                return true;
            }
            return false;
        }

        public bool Got(ref byte[] v)
        {
            int ord = ordinal++;
            try
            {
                if (!reader.IsDBNull(ord))
                {
                    int len;
                    if ((len = (int)reader.GetBytes(ord, 0, null, 0, 0)) > 0)
                    {
                        // get the number of bytes that are available to read.
                        v = new byte[len];
                        reader.GetBytes(ord, 0, v, 0, len); // read data into the buffer
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public bool Got<T>(ref T v, int x = -1) where T : IPersist, new()
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                string s = reader.GetString(ord);

                // TODO
                return true;
            }
            return false;
        }

        public bool Got(ref JObj v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                string strv = reader.GetString(ord);
                JStrParse parse = new JStrParse(strv);
                v = (JObj)parse.Parse();
                return true;
            }
            return false;
        }

        public bool Got(ref JArr v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                string strv = reader.GetString(ord);
                JStrParse parse = new JStrParse(strv);
                v = (JArr)parse.Parse();
                return true;
            }
            return false;
        }

        public bool Got(ref short[] v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<short[]>(ord);
                return true;
            }
            return false;
        }

        public bool Got(ref int[] v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<int[]>(ord);
                return true;
            }
            return false;
        }

        public bool Got(ref long[] v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<long[]>(ord);
                return true;
            }
            return false;
        }

        public bool Got(ref string[] v)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<string[]>(ord);
                return true;
            }
            return false;
        }

        public bool Got<T>(ref T[] v, int x = -1) where T : IPersist, new()
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                string strv = reader.GetString(ord);
                JStrParse parse = new JStrParse(strv);
                JArr jarr = (JArr)parse.Parse();
                v = new T[jarr.Count];
                for (int i = 0; i < jarr.Count; i++)
                {
                    JObj jobj = (JObj)jarr[i];
                    v[i].Load(jobj, x);
                }
                return true;
            }
            return false;
        }

        // SOURCE


        public bool Got(string name, ref bool v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetBoolean(ord);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref short v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt16(ord);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref int v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt32(ord);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref long v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt64(ord);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref decimal v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetDecimal(ord);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref DateTime v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetDateTime(ord);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref char[] v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<char[]>(ord);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref string v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetString(ord);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref byte[] v)
        {
            int ord = reader.GetOrdinal(name);
            try
            {
                if (!reader.IsDBNull(ord))
                {
                    int len;
                    if ((len = (int)reader.GetBytes(ord, 0, null, 0, 0)) > 0)
                    {
                        // get the number of bytes that are available to read.
                        v = new byte[len];
                        reader.GetBytes(ord, 0, v, 0, len); // read data into the buffer
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public bool Got<T>(string name, ref T v, int x = -1) where T : IPersist, new()
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string strv = reader.GetString(ord);
                JStrParse parse = new JStrParse(strv);
                JObj jobj = (JObj)parse.Parse();
                v = new T();
                v.Load(jobj, x);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref JObj v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string strv = reader.GetString(ord);
                JStrParse parse = new JStrParse(strv);
                v = (JObj)parse.Parse();
                return true;
            }
            return false;
        }

        public bool Got(string name, ref JArr v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string strv = reader.GetString(ord);
                JStrParse parse = new JStrParse(strv);
                v = (JArr)parse.Parse();
                return true;
            }
            return false;
        }

        public bool Got(string name, ref short[] v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<short[]>(ord);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref int[] v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<int[]>(ord);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref long[] v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<long[]>(ord);
                return true;
            }
            return false;
        }

        public bool Got(string name, ref string[] v)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<string[]>(ord);
                return true;
            }
            return false;
        }

        public bool Got<T>(string name, ref T[] v, int x = -1) where T : IPersist, new()
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string strv = reader.GetString(ord);
                JStrParse parse = new JStrParse(strv);
                JArr jarr = (JArr)parse.Parse();
                v = new T[jarr.Count];
                for (int i = 0; i < jarr.Count; i++)
                {
                    JObj jobj = (JObj)jarr[i];
                    v[i].Load(jobj, x);
                }
                return true;
            }
            return false;
        }


        //
        // sends an event to a target service
        //


        public void PostHorizontalEvent<E>(string topic, int verb, E msg) where E : IPersist
        {
        }

        public void PostVerticalEvent<E>(string topic, int verb, E msg) where E : IPersist
        {
        }


        public void SendEvent<T>(string topic, string filter, T @event) where T : IPersist
        {
            // convert message to byte buffer
            JContent b = new JContent(16 * 1024);
            @event.Save(b);

            Execute("INSERT INTO mq (topic, filter, message) VALUES (@topic, @filter, @message)", p =>
            {
                p.Put("@topic", topic);
                p.Put("@filter", filter);
                // p.Put("@message", new ArraySegment<byte>(b.Buffer, 0, b.Length));
            });
        }

        public void Dispose()
        {
            if (!disposed)
            {
                reader?.Dispose();
                command.Dispose();
                connection.Dispose();
                // indicate that the instance has been disposed.
                disposed = true;
            }
        }

    }
}