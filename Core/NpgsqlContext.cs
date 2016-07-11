using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    public class NpgsqlContext : IInput, IOutput, IDisposable
    {
        private NpgsqlConnection connection;

        private NpgsqlCommand command;

        private NpgsqlTransaction transaction;

        private NpgsqlParameterCollection _parameters;

        private NpgsqlDataReader _reader;


        bool disposed;

        public void BeginTransaction()
        {
            if (transaction == null)
            {
                transaction = connection.BeginTransaction();
                command.Transaction = transaction;
            }
        }

        public void BeginTransaction(IsolationLevel level)
        {
            if (transaction == null)
            {
                transaction = connection.BeginTransaction(level);
                command.Transaction = transaction;
            }
        }

        public void CommitTransaction()
        {
            if (transaction != null)
            {
                transaction.Commit();
                command.Transaction = null;
                transaction = null;
            }
        }

        public void RollbackTransaction()
        {
            if (transaction != null)
            {
                transaction.Rollback();
                command.Transaction = null;
                transaction = null;
            }
        }

        public int DoNonQuery(string cmdtext, Action<IOutput> parameters)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            // add parameters
            parameters?.Invoke(this);

            // execute
            return command.ExecuteNonQuery();
        }

        public void Command(string sql, params object[] args)
        {
            command = new NpgsqlCommand(sql, connection, transaction);
        }

        //
        //

        bool IInput.GotStart()
        {
            throw new NotImplementedException();
        }

        bool IInput.GotEnd()
        {
            throw new NotImplementedException();
        }

        void IOutput.PutStart()
        {
            throw new NotImplementedException();
        }

        void IOutput.PutEnd()
        {
            throw new NotImplementedException();
        }

        ///
        ///  TURNING TARGET
        ///
        ///
        public void Put(string name, int value)
        {
            _parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = value
            });
        }

        public void Put(string name, decimal value)
        {
            _parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = value
            });
        }

        public void Put(string name, string value)
        {
            _parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Varchar)
            {
                Value = value
            });
        }

        public void Put<T>(string name, List<T> value) where T : IDump
        {
            JsonString json = new JsonString();
            for (int i = 0; i < value.Count; i++)
            {
                if (i > 0)
                {
                    json.PutEnd();// comma
                }
                json.PutStart();
                T d = value[i];
                d.To(json);
                json.PutEnd();
            }
        }

        public bool Got(string name, out int value)
        {
            int ordinal = _reader.GetOrdinal(name);
            if (!_reader.IsDBNull(ordinal))
            {
                value = _reader.GetInt32(ordinal);
                return true;
            }
            value = 0;
            return false;
        }

        public bool Got(string name, out decimal value)
        {
            int ordinal = _reader.GetOrdinal(name);
            if (!_reader.IsDBNull(ordinal))
            {
                value = _reader.GetDecimal(ordinal);
                return true;
            }
            value = 0;
            return false;
        }

        public bool Got(string name, out string value)
        {
            int ordinal = _reader.GetOrdinal(name);
            if (!_reader.IsDBNull(ordinal))
            {
                value = _reader.GetString(ordinal);
                return true;
            }
            value = null;
            return false;
        }

        public bool Got<T>(string name, out List<T> value) where T : IDump
        {
            if (Got(name, out value))
            {
//                Json son = new Json();
            }
            return false;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                _reader?.Dispose();
                command.Dispose();
                connection.Dispose();
                // indicate that the instance has been disposed.
                disposed = true;
            }
        }
    }
}