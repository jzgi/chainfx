using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    public class SqlContext : IDisposable, IParameterSet, IResultSet
    {
        readonly NpgsqlConnection connection;

        readonly NpgsqlCommand command;

        private NpgsqlTransaction transact;

        private NpgsqlDataReader reader;

        bool disposed;


        public SqlContext(NpgsqlConnectionStringBuilder builder)
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

        public bool QueryOne(string cmdtext, Action<IParameterSet> ps)
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

        public bool ReadRow<T>(ref T value) where T : ISerial, new()
        {
            if (value == null)
            {
                value = new T();
            }
            value.ReadFrom(this);
            return true;
        }

        public T ReadRow<T>() where T : ISerial, new()
        {
            T value = new T();
            value.ReadFrom(this);
            return value;
        }

        public int Execute(string cmdtext, Action<IParameterSet> ps)
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

        ///
        ///  TURNING TARGET
        ///
        public void Add(string name, int value)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = value
            });
        }

        public void Add(string name, decimal value)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = value
            });
        }

        public void Add(string name, string value)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Varchar)
            {
                Value = value
            });
        }

        public void Add(string name, ArraySegment<byte> value)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea)
            {
                Value = value
            });
        }


        //
        // READER
        //

        private int ordinal;


        public bool Read(ref int value)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetInt32(ord);
                return true;
            }
            return false;
        }

        public bool Read(ref string value)
        {
            int ord = ordinal++;
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetString(ord);
                return true;
            }
            return false;
        }


        public bool Read(string name, ref string value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetString(ord);
                return true;
            }
            return false;
        }

        public bool Read<T>(string name, ref List<T> value)
        {
            if (Read(name, ref value))
            {
                //                Json son = new Json();
            }
            return false;
        }


        public bool Read(string name, ref bool value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetBoolean(ord);
                return true;
            }
            return false;
        }

        public bool Read(string name, ref short value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetInt16(ord);
                return true;
            }
            return false;
        }

        public bool Read(string name, ref int value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetInt32(ord);
                return true;
            }
            return false;
        }

        public bool Read(string name, ref decimal value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetDecimal(ord);
                return true;
            }
            return false;
        }

        public bool Read(string name, ref DateTime value)
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                value = reader.GetDateTime(ord);
                return true;
            }
            return false;
        }

        public bool Read<T>(string name, ref T value) where T : ISerial, new()
        {
            int ord = reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string str = reader.GetString(ord);
                JsonText json = new JsonText(str);
                if (value == null)
                {
                    value = new T();
                }
                value.ReadFrom(json);
                return true;
            }
            return false;
        }

        public bool Read(string name, ref List<string> value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref string[] value)
        {
            throw new NotImplementedException();
        }

        bool ISerialReader.Read<T>(string name, ref List<T> value)
        {
            return Read(name, ref value);
        }


        //
        // WRITERS
        //

        public void Write(string name, bool value)
        {
            throw new NotImplementedException();
        }

        public bool Read<K, V>(string name, ref Dictionary<K, V> value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, short value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, int value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, decimal value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, DateTime value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, string value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, ISerial value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(string name, List<T> list)
        {
            throw new NotImplementedException();
        }

        public void Write<V>(string name, Dictionary<string, V> dict)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, params string[] array)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, params ISerial[] array)
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
            BJsonContent b = new BJsonContent(null);
            @event.WriteTo(b);

            Execute("INSERT INTO mq (topic, filter, message) VALUES (@topic, @filter, @message)", p =>
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
                command.Dispose();
                connection.Dispose();
                // indicate that the instance has been disposed.
                disposed = true;
            }
        }
    }
}