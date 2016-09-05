using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    public class SqlContext : IDisposable, IParameterCollection, IResultSet
    {
        readonly NpgsqlConnection connection;

        private NpgsqlCommand cmd;

        private NpgsqlTransaction transact;

        private NpgsqlParameterCollection @params;

        private NpgsqlDataReader reader;


        public SqlContext(NpgsqlConnectionStringBuilder builder)
        {
            connection = new NpgsqlConnection(builder);
        }

        bool disposed;

        public void BeginTransaction()
        {
            if (transact == null)
            {
                transact = connection.BeginTransaction();
                cmd.Transaction = transact;
            }
        }

        public void BeginTransaction(IsolationLevel level)
        {
            if (transact == null)
            {
                transact = connection.BeginTransaction(level);
                cmd.Transaction = transact;
            }
        }

        public void CommitTransaction()
        {
            if (transact != null)
            {
                transact.Commit();
                cmd.Transaction = null;
                transact = null;
            }
        }

        public void RollbackTransaction()
        {
            if (transact != null)
            {
                transact.Rollback();
                cmd.Transaction = null;
                transact = null;
            }
        }

        public void DoQuery(string cmdtext, Action<IParameterCollection> @params)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            // add parameters
            @params?.Invoke(this);

            reader = cmd.ExecuteReader();
        }

        public T Read<T>(Func<IResultSet, T> garther) where T : new()
        {
            return garther(this);
        }

        public int DoNonQuery(string cmdtext, Action<IParameterCollection> parameters)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            // add parameters
            parameters?.Invoke(this);

            // execute
            return cmd.ExecuteNonQuery();
        }

        public void Command(string sql, params object[] args)
        {
            cmd = new NpgsqlCommand(sql, connection, transact);
        }

        //
        //


        ///
        ///  TURNING TARGET
        ///
        public void Add(string name, int value)
        {
            @params.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = value
            });
        }

        public void Add(string name, decimal value)
        {
            @params.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = value
            });
        }

        public void Add(string name, string value)
        {
            @params.Add(new NpgsqlParameter(name, NpgsqlDbType.Varchar)
            {
                Value = value
            });
        }

        public void Add(string name, ArraySegment<byte> value)
        {
            @params.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea)
            {
                Value = value
            });
        }

        public bool Got(string name, ref int value)
        {
            int ordinal = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ordinal))
            {
                value = reader.GetInt32(ordinal);
                return true;
            }
            value = 0;
            return false;
        }

        public bool Got(string name, ref decimal value)
        {
            int ordinal = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ordinal))
            {
                value = reader.GetDecimal(ordinal);
                return true;
            }
            value = 0;
            return false;
        }

        public bool Got(string name, ref string value)
        {
            int ordinal = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ordinal))
            {
                value = reader.GetString(ordinal);
                return true;
            }
            value = null;
            return false;
        }

        public bool Got<T>(string name, ref List<T> value)
        {
            if (Got(name, ref value))
            {
//                Json son = new Json();
            }
            return false;
        }


        public bool Got(ref int value)
        {
            throw new NotImplementedException();
        }

        public bool Got(ref string value)
        {
            throw new NotImplementedException();
        }

        ///
        /// sends an event to a target service
        ///
        public void PostHorizontalEvent<E>(string topic, int verb, E msg) where E : ISerial
        {
        }

        public void PostVerticalEvent<E>(string topic, int verb, E msg) where E : ISerial
        {
        }


        public void SendEvent<T>(string topic, string filter, T @event) where T : ISerial
        {
            // convert message to byte buffer
            BJsonContent b = new BJsonContent(null);
            @event.WriteTo(b);

            DoNonQuery("INSERT INTO mq (topic, filter, message) VALUES (@topic, @filter, @message)", p =>
            {
                p.Add("@topic", topic);
                p.Add("@filter", filter);
                p.Add("@message", new ArraySegment<byte>(b.Buffer, 0, b.Count));
            });
        }

        public void Dispose()
        {
            if (!disposed)
            {
                reader?.Dispose();
                cmd.Dispose();
                connection.Dispose();
                // indicate that the instance has been disposed.
                disposed = true;
            }
        }
    }
}