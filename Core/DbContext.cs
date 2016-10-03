using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using NpgsqlTypes;

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

        public bool Map<T>(ref T po, int x = -1) where T : IPersist, new()
        {
            if (po == null)
            {
                po = new T();
            }
            po.Load(this, x);
            return true;
        }

        public bool Map<T>(ref List<T> lst, int x = -1) where T : IPersist, new()
        {
            if (lst == null)
            {
                lst = new List<T>(32);
            }
            while (NextRow())
            {
                T po = new T();
                if (Map(ref po, x))
                {
                    lst.Add(po);
                }
            }
            return true;
        }

        // current column ordinal
        int colord;


        public bool Get(ref bool value)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetBoolean(ord);
                return true;
            }
            return false;
        }

        public bool Get(ref short value)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetInt16(ord);
                return true;
            }
            return false;
        }

        public bool Get(ref int value)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetInt32(ord);
                return true;
            }
            return false;
        }

        public bool Get(ref long value)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetInt64(ord);
                return true;
            }
            return false;
        }

        public bool Get(ref decimal value)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetDecimal(ord);
                return true;
            }
            return false;
        }

        public bool Get(ref DateTime value)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetDateTime(ord);
                return true;
            }
            return false;
        }

        public bool Get(ref string value)
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetString(ord);
                return true;
            }
            return false;
        }

        public bool Get<T>(ref T value, int x = -1) where T : IPersist, new()
        {
            int ord = colord++;
            if (!reader.IsDBNull(ord))
            {
                string v = reader.GetString(ord);

                // TODO
                return true;
            }
            return false;
        }

        public bool Get<T>(ref List<T> value, int x = -1) where T : IPersist, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(ref byte[] value)
        {
            throw new NotImplementedException();
        }

        public bool Get(ref Obj value)
        {
            throw new NotImplementedException();
        }

        public bool Get(ref Arr value)
        {
            throw new NotImplementedException();
        }

        // SOURCE

        public bool Get(string name, ref bool value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetBoolean(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref short value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetInt16(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref int value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetInt32(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref long value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref decimal value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetDecimal(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref DateTime value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetDateTime(ord);
                return true;
            }
            return false;
        }

        public bool Get(string name, ref string value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetString(ord);
                return true;
            }
            return false;
        }

        public bool Get<T>(string name, ref T value, int x = -1) where T : IPersist, new()
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string str = reader.GetString(ord);
                // JsonText json = new JsonText(str);
                // if (value == null) value = new T();
                // value.Load(json, x);
                return true;
            }
            return false;
        }

        public bool Get<T>(string name, ref List<T> value, int x = -1) where T : IPersist, new()
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref byte[] value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref Obj value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref Arr value)
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
            @event.Save(b, 0);

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