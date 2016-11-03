using System;
using System.Collections.Generic;
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

        void Clear()
        {
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
            parameters.Clear();
            ordinal = 0;
        }

        public bool QueryA(string cmdtext, Action<DbParameters> p = null)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            command.CommandText = cmdtext;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(parameters);
                command.Prepare();
            }
            reader = command.ExecuteReader();
            return reader.Read();
        }

        public bool Query(string cmdtext, Action<DbParameters> p = null)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            command.CommandText = cmdtext;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(parameters);
                command.Prepare();
            }
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

        public int Execute(string cmdtext, Action<DbParameters> p = null)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            command.CommandText = cmdtext;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(parameters);
                command.Prepare();
            }
            return command.ExecuteNonQuery();
        }

        public object Scalar(string cmdtext, Action<DbParameters> p = null)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            command.CommandText = cmdtext;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(parameters);
                command.Prepare();
            }
            return command.ExecuteScalar();
        }

        //
        // RESULTSET
        //

        public P ToObj<P>(byte x = 0xff) where P : IPersist, new()
        {
            P obj = new P();
            obj.Load(this, x);
            return obj;
        }


        public P[] ToArr<P>(byte x = 0xff) where P : IPersist, new()
        {
            List<P> lst = new List<P>(64);
            while (NextRow())
            {
                P obj = new P();
                obj.Load(this, x);
                lst.Add(obj);
            }
            return lst.ToArray();
        }


        // current column ordinal
        int ordinal;

        public bool Get(string name, ref bool v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetBoolean(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt16(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt32(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt64(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetDecimal(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref Number v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                decimal decv = reader.GetDecimal(ord);
                // TODO
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetDateTime(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref char[] v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<char[]>(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetString(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
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

        public bool Get(string name, ref ArraySegment<byte>? v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            try
            {
                if (!reader.IsDBNull(ord))
                {
                    int len;
                    if ((len = (int)reader.GetBytes(ord, 0, null, 0, 0)) > 0)
                    {
                        byte[] arr = BufferPool.Borrow(len);
                        reader.GetBytes(ord, 0, arr, 0, len); // read data into the buffer
                        v = new ArraySegment<byte>(arr, 0, len);
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public bool Get<P>(string name, ref P v, byte x = 0xff) where P : IPersist, new()
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string str = reader.GetString(ord);
                JTextParse par = new JTextParse(str);
                JObj jo = (JObj)par.Parse();
                v = new P();
                v.Load(jo, x);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref JObj v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string str = reader.GetString(ord);
                JTextParse par = new JTextParse(str);
                v = (JObj)par.Parse();
                return true;
            }
            return false;
        }

        public bool Get(string name, ref JArr v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string str = reader.GetString(ord);
                JTextParse par = new JTextParse(str);
                v = (JArr)par.Parse();
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short[] v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<short[]>(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<int[]>(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<long[]>(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<string[]>(ord);
                return true;
            }
            return false;
        }

        public bool Get<P>(string name, ref P[] v, byte x = 0xff) where P : IPersist, new()
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string str = reader.GetString(ord);
                JTextParse par = new JTextParse(str);
                JArr ja = (JArr)par.Parse();
                int len = ja.Count;
                v = new P[len];
                for (int i = 0; i < len; i++)
                {
                    JObj jo = (JObj)ja[i];
                    P obj = new P();
                    obj.Load(jo, x);
                    v[i] = obj;
                }
                return true;
            }
            return false;
        }


        //
        // MESSAGING
        //

        public void Publish(string topic, string part, Action<JContent> a)
        {
            // convert message to byte buffer
            JContent jcont = new JContent(8 * 1024);
            a?.Invoke(jcont);

            Execute("INSERT INTO mq (topic, filter, message) VALUES (@topic, @filter, @message)", p =>
            {
                p.Put("@topic", topic);
                p.Put("@filter", part);
                // p.Put("@message", new ArraySegment<byte>(b.Buffer, 0, b.Length));
            });
        }

        public void Publish<P>(string topic, string part, P obj) where P : IPersist
        {
            Publish(topic, part, jcont => jcont.PutObj(obj));
        }

        public void Publish<P>(string topic, string part, P[] arr) where P : IPersist
        {
            Publish(topic, part, jcont => jcont.PutArr(arr));
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