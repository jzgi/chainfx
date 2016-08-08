using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    public class NpgsqlContext : IDataInput, IDataOutput, IDisposable
    {
        private NpgsqlConnection _conn;

        private NpgsqlCommand _cmd;

        private NpgsqlTransaction _transact;

        private NpgsqlParameterCollection _params;

        private NpgsqlDataReader _reader;


	    public NpgsqlContext(NpgsqlConnectionStringBuilder builder)
	    {
		    _conn = new NpgsqlConnection(builder);
	    }

        bool disposed;

        public void BeginTransaction()
        {
            if (_transact == null)
            {
                _transact = _conn.BeginTransaction();
                _cmd.Transaction = _transact;
            }
        }

        public void BeginTransaction(IsolationLevel level)
        {
            if (_transact == null)
            {
                _transact = _conn.BeginTransaction(level);
                _cmd.Transaction = _transact;
            }
        }

        public void CommitTransaction()
        {
            if (_transact != null)
            {
                _transact.Commit();
                _cmd.Transaction = null;
                _transact = null;
            }
        }

        public void RollbackTransaction()
        {
            if (_transact != null)
            {
                _transact.Rollback();
                _cmd.Transaction = null;
                _transact = null;
            }
        }

        public int DoNonQuery(string cmdtext, Action<IDataOutput> parameters)
        {
            if (_conn.State != ConnectionState.Open)
            {
                _conn.Open();
            }
            // add parameters
            parameters?.Invoke(this);

            // execute
            return _cmd.ExecuteNonQuery();
        }

        public void Command(string sql, params object[] args)
        {
            _cmd = new NpgsqlCommand(sql, _conn, _transact);
        }

        //
        //

        public bool GotStart()
        {
	        return true;
        }

        public bool GotEnd()
        {
	        return true;
        }

        public void PutStart()
        {
        }

        public void PutEnd()
        {
        }

        ///
        ///  TURNING TARGET
        ///
        ///
        public void Put(string name, int value)
        {
            _params.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = value
            });
        }

        public void Put(string name, decimal value)
        {
            _params.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = value
            });
        }

        public void Put(string name, string value)
        {
            _params.Add(new NpgsqlParameter(name, NpgsqlDbType.Varchar)
            {
                Value = value
            });
        }

        public void Put<T>(string name, List<T> value) where T : IData
        {
            JsonCodec json = new JsonCodec(1024);
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

        public bool Got<T>(string name, out List<T> value) where T : IData
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
                _cmd.Dispose();
                _conn.Dispose();
                // indicate that the instance has been disposed.
                disposed = true;
            }
        }
    }
}