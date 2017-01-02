using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// An environment for database operations based on current service.
    ///
    public class DbContext : IDisposable, IResultSet
    {
        readonly string shard;

        readonly NpgsqlConnection connection;

        readonly NpgsqlCommand command;

        readonly DbParameters parameters;

        NpgsqlTransaction transact;

        NpgsqlDataReader reader;

        bool disposed;


        internal DbContext(string shard, NpgsqlConnectionStringBuilder builder)
        {
            this.shard = shard;
            connection = new NpgsqlConnection(builder);
            command = new NpgsqlCommand();
            parameters = new DbParameters(command.Parameters);
            command.Connection = connection;
        }

        public string Shard => shard;

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

        public bool QueryA(DbSql sql, Action<DbParameters> p = null)
        {
            bool v = QueryA(sql.ToString(), p);
            BufferUtility.Return(sql);
            return v;
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

        public bool Query(DbSql sql, Action<DbParameters> p = null)
        {
            bool v = Query(sql.ToString(), p);
            BufferUtility.Return(sql);
            return v;
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

        public int Execute(DbSql sql, Action<DbParameters> p = null)
        {
            int v = Execute(sql.ToString(), p);
            BufferUtility.Return(sql);
            return v;
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

        public object Scalar(DbSql sql, Action<DbParameters> p = null)
        {
            object v = Scalar(sql.ToString(), p);
            BufferUtility.Return(sql);
            return v;
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

        public D ToDat<D>(byte bits = 0) where D : IDat, new()
        {
            D dat = new D();
            dat.Load(this, bits);

            // add shard if any
            IShardable shardable = dat as IShardable;
            if (shardable != null)
            {
                shardable.Shard = shard;
            }

            return dat;
        }


        public D[] ToDats<D>(byte bits = 0) where D : IDat, new()
        {
            List<D> lst = new List<D>(64);
            while (NextRow())
            {
                D dat = new D();
                dat.Load(this, bits);

                // add shard if any
                IShardable shardable = dat as IShardable;
                if (shardable != null)
                {
                    shardable.Shard = shard;
                }

                lst.Add(dat);
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

        public bool Get(string name, ref double v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetDouble(ord);
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

        public bool Get(string name, ref NpgsqlPoint v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                v = reader.GetFieldValue<NpgsqlPoint>(ord);
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
                    if ((len = (int) reader.GetBytes(ord, 0, null, 0, 0)) > 0)
                    {
                        // get the number of bytes that are available to read.
                        v = new byte[len];
                        reader.GetBytes(ord, 0, v, 0, len); // read data into the buffer
                        return true;
                    }
                }
            }
            catch
            {
            }

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
                    if ((len = (int) reader.GetBytes(ord, 0, null, 0, 0)) > 0)
                    {
                        byte[] buf = BufferUtility.BorrowByteBuf(len);
                        reader.GetBytes(ord, 0, buf, 0, len); // read data into the buffer
                        v = new ArraySegment<byte>(buf, 0, len);
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        public bool Get<D>(string name, ref D v, byte bits = 0) where D : IDat, new()
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string str = reader.GetString(ord);
                JsonParse p = new JsonParse(str);
                JObj jobj = (JObj) p.Parse();
                v = new D();
                v.Load(jobj, bits);

                // add shard if any
                IShardable shardable = v as IShardable;
                if (shardable != null)
                {
                    shardable.Shard = shard;
                }
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
                JsonParse p = new JsonParse(str);
                v = (JObj) p.Parse();
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
                JsonParse p = new JsonParse(str);
                v = (JArr) p.Parse();
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

        public bool Get<D>(string name, ref D[] v, byte bits = 0) where D : IDat, new()
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            if (!reader.IsDBNull(ord))
            {
                string str = reader.GetString(ord);
                JsonParse p = new JsonParse(str);
                JArr jarr = (JArr) p.Parse();
                int len = jarr.Count;
                v = new D[len];
                for (int i = 0; i < len; i++)
                {
                    JObj jobj = jarr[i];
                    D dat = new D();
                    dat.Load(jobj, bits);

                    // add shard if any
                    IShardable shardable = dat as IShardable;
                    if (shardable != null)
                    {
                        shardable.Shard = shard;
                    }

                    v[i] = dat;
                }
                return true;
            }
            return false;
        }


        //
        // MESSAGING
        //

        public void QueueEvent<D>(string name, string shard, D dat) where D : IDat
        {
            QueueEvent(name, shard, jcont => jcont.Put(null, dat));
        }

        public void QueueEvent<D>(string name, string shard, D[] dats) where D : IDat
        {
            QueueEvent(name, shard, jcont => jcont.Put(null, dats));
        }

        public void QueueEvent(string name, string shard, Action<JsonContent> a)
        {
            // convert message to byte buffer
            JsonContent cont = new JsonContent(true, true);
            a?.Invoke(cont);

            Execute("INSERT INTO eq (name, shard, body) VALUES (@1, @2, @3)", p =>
            {
                p.Put(name);
                p.Put(shard);
                p.Put(cont.ToBytesSeg());
            });
            BufferUtility.Return(cont);
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