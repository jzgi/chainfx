using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Npgsql;

namespace Greatbone.Core
{
    public class DbContext : IDisposable, IResultSet
    {
        readonly NpgsqlConnection connection;

        readonly NpgsqlCommand command;

        private NpgsqlTransaction transact;

        private NpgsqlDataReader reader;

        bool disposed;


        public DbContext(NpgsqlConnectionStringBuilder builder)
        {
            connection = new NpgsqlConnection(builder);
            command = new NpgsqlCommand();
            command.Connection = connection;
        }

        public void BeginTransaction()
        {
            if (transact == null)
            {
                transact = connection.BeginTransaction();
                command.Transaction = transact;
            }
        }

        public void BeginTransaction(IsolationLevel level)
        {
            if (transact == null)
            {
                transact = connection.BeginTransaction(level);
                command.Transaction = transact;
            }
        }

        public void CommitTransaction()
        {
            if (transact != null)
            {
                transact.Commit();
                command.Transaction = null;
                transact = null;
            }
        }

        public void RollbackTransaction()
        {
            if (transact != null)
            {
                transact.Rollback();
                command.Transaction = null;
                transact = null;
            }
        }

        public bool QueryA(string cmdtext, Action<DbParameterCollection> ps)
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
            command.Parameters.Clear();
            ps?.Invoke(command.Parameters);

            reader = command.ExecuteReader();
            return reader.Read();
        }

        public bool Query(string cmdtext, Action<DbParameterCollection> ps)
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
            command.Parameters.Clear();
            ps?.Invoke(command.Parameters);

            reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public bool NextRow()
        {
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
            if (reader == null)
            {
                return false;
            }
            return reader.NextResult();
        }

        public int Execute(string cmdtext, Action<DbParameterCollection> ps)
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
            command.Parameters.Clear();
            ps?.Invoke(command.Parameters);

            return command.ExecuteNonQuery();
        }

        //
        // RESULTSET
        //

        private int ordinal;


        public bool Get(out int value)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetInt32(ord);
                return true;
            }
            value = 0;
            return false;
        }

        public bool Get(out string value)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetString(ord);
                return true;
            }
            value = null;
            return false;
        }


        public bool Get(string name, out string value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetString(ord);
                return true;
            }
            value = null;
            return false;
        }

        public bool Get<T>(string name, out List<T> value)
        {
            if (Get(name, out value))
            {
                //                Json son = new Json();
            }
            value = null;
            return false;
        }


        public bool Get(string name, out bool value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetBoolean(ord);
                return true;
            }
            value = false;
            return false;
        }

        public bool Get(string name, out short value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetInt16(ord);
                return true;
            }
            value = 0;
            return false;
        }

        public bool Get(string name, out int value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetInt32(ord);
                return true;
            }
            value = 0;
            return false;
        }

        public bool Get(string name, out decimal value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetDecimal(ord);
                return true;
            }
            value = 0;
            return false;
        }

        public bool Get(string name, out DateTime value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetDateTime(ord);
                return true;
            }
            value = default(DateTime);
            return false;
        }

        public bool Get<T>(string name, out T value) where T : ISerial, new()
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string str = reader.GetString(ord);
                JsonText json = new JsonText(str);
                value = new T();
                value.ReadFrom(json);
                return true;
            }
            value = default(T);
            return false;
        }

        public bool Get(string name, ref List<string> value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string[] value)
        {
            throw new NotImplementedException();
        }

        public bool Get<K, V>(string name, ref Dictionary<K, V> value)
        {
            throw new NotImplementedException();
        }


        //
        // sends an event to a target service
        //


        public void PostHorizontalEvent<E>(string topic, int verb, E msg) where E : ISerial
        {
        }

        public void PostVerticalEvent<E>(string topic, int verb, E msg) where E : ISerial
        {
        }


        public void SendEvent<T>(string topic, string filter, T @event) where T : ISerial
        {
            // convert message to byte buffer
            JsobContent b = new JsobContent(16 * 1024);
            @event.WriteTo(b);

            Execute("INSERT INTO mq (topic, filter, message) VALUES (@topic, @filter, @message)", p =>
            {
                p.Set("@topic", topic);
                p.Set("@filter", filter);
                p.Set("@message", new ArraySegment<byte>(b.Buffer, 0, b.Length));
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