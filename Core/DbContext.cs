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
            colord = 0;
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
            colord = 0;
            reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public bool NextRow()
        {
            colord = 0; // reset column ordinal

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
            colord = 0; // reset column ordinal

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
            colord = 0;
            return command.ExecuteNonQuery();
        }

        //
        // RESULTSET
        //

        public T Get<T>() where T : IPersist, new()
        {
            T obj = new T();
            obj.Load(this);
            return obj;
        }

        public List<T> GetList<T>() where T : IPersist, new()
        {
            List<T> lst = new List<T>(32);

            while (NextRow())
            {
                T obj = new T();
                obj.Load(this);
                lst.Add(obj);
            }
            return lst;
        }

        // current column ordinal
        int colord;


        public bool Got(out bool v, bool def = false)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetBoolean(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got(out short v, short def = 0)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt16(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got(out int v, int def = 0)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt32(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got(out long v, long def = 0)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt64(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got(out decimal v, decimal def = 0)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetDecimal(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got(out DateTime v, DateTime def = default(DateTime))
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetDateTime(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got(out string v, string def = null)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetString(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got<T>(out T v, T def = default(T)) where T : IPersist, new()
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                string s = reader.GetString(ord);

                // TODO
                v = def;
                return true;
            }
            v = def;
            return false;
        }

        public bool Got<T>(out List<T> v, List<T> def = null) where T : IPersist, new()
        {
            throw new NotImplementedException();
        }

        public bool Got(out byte[] v, byte[] def = null)
        {
            throw new NotImplementedException();
        }

        public bool Got(out Obj v, Obj def = null)
        {
            throw new NotImplementedException();
        }

        public bool Got(out Arr v, Arr def = null)
        {
            throw new NotImplementedException();
        }

        // SOURCE


        public bool Got(string name, out bool v, bool def = false)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetBoolean(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got(string name, out short v, short def = 0)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt16(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got(string name, out int v, int def = 0)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt32(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got(string name, out long v, long def = 0)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetInt64(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got(string name, out decimal v, decimal def = 0)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetDecimal(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got(string name, out DateTime v, DateTime def = default(DateTime))
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetDateTime(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got(string name, out string v, string def = null)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetString(ord);
                return true;
            }
            v = def;
            return false;
        }

        public bool Got<T>(string name, out T v, T def = default(T)) where T : IPersist, new()
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {

                string str = reader.GetString(ord);
                // JsonText json = new JsonText(str);
                // if (value == null) value = new T();
                // value.Load(json, x);
                v = def;
                return true;
            }
            v = def;
            return false;
        }

        public bool Got<T>(string name, out List<T> v, List<T> def = null) where T : IPersist, new()
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, out byte[] v, byte[] def = null)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, out Obj v, Obj def = null)
        {
            throw new NotImplementedException();
        }

        public bool Got(string name, out Arr v, Arr def = null)
        {
            throw new NotImplementedException();
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
            JsonContent b = new JsonContent(16 * 1024);
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