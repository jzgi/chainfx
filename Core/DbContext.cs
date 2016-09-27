using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    public class DbContext : IDisposable, IResultSet, IParameterSet
    {
        readonly NpgsqlConnection connection;

        readonly NpgsqlCommand command;

        readonly NpgsqlParameterCollection parameters;

        private NpgsqlTransaction transact;

        private NpgsqlDataReader reader;

        bool disposed;


        public DbContext(NpgsqlConnectionStringBuilder builder)
        {
            connection = new NpgsqlConnection(builder);
            command = new NpgsqlCommand();
            parameters = command.Parameters;
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

        public bool QueryA(string cmdtext, Action<IParameterSet> ps)
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
            ps?.Invoke(this);

            reader = command.ExecuteReader();
            return reader.Read();
        }

        public bool Query(string cmdtext, Action<IParameterSet> ps)
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
            ps?.Invoke(this);

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

        public int Execute(string cmdtext, Action<IOut> ps)
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
            ps?.Invoke(this);

            return command.ExecuteNonQuery();
        }

        //
        // RESULTSET
        //

        private int ordinal;


        public bool Get(ref int value)
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

        public bool Get(ref string value)
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

        public bool Get<T>(string name, ref List<T> value)
        {
            if (Get(name, ref value))
            {
                //                Json son = new Json();
            }
            return false;
        }


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

        public bool Get<T>(string name, ref T value) where T : IData, new()
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string str = reader.GetString(ord);
                JsonText json = new JsonText(str);
                value = new T();
                // value.From(json);
                return true;
            }
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


        public void PostHorizontalEvent<E>(string topic, int verb, E msg) where E : IData
        {
        }

        public void PostVerticalEvent<E>(string topic, int verb, E msg) where E : IData
        {
        }


        public void SendEvent<T>(string topic, string filter, T @event) where T : IData
        {
            // convert message to byte buffer
            JsobContent b = new JsobContent(16 * 1024);
            @event.Write(b);

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

        public bool Get(string name, ref long value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref byte[] value)
        {
            throw new NotImplementedException();
        }

        public bool Get<T>(string name, ref Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }


        public IOut Put(string name, bool value)
        {
            parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Boolean)
            {
                Value = value
            });
            return this;
        }

        public IOut Put(string name, short value)
        {
            throw new NotImplementedException();
        }

        public IOut Put(string name, int value)
        {
            parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = value
            });
            return this;
        }

        public IOut Put(string name, long value)
        {
            parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Bigint)
            {
                Value = value
            });
            return this;
        }

        public IOut Put(string name, decimal value)
        {
            parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = value
            });
            return this;
        }

        public IOut Put(string name, DateTime value)
        {
            parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Timestamp)
            {
                Value = value
            });
            return this;
        }

        public IOut Put(string name, char[] value)
        {
            throw new NotImplementedException();
        }

        public IOut Put(string name, string value)
        {
            throw new NotImplementedException();
        }

        public IOut Put<T>(string name, T value) where T : IData
        {
            throw new NotImplementedException();
        }

        public IOut Put(string name, byte[] value)
        {
            throw new NotImplementedException();
        }

        public IOut Put<T>(string name, List<T> value)
        {
            throw new NotImplementedException();
        }

        public IOut Put<T>(string name, Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }
    }
}