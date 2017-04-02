using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// An environment for database operations based on current service.
    ///
    public class DbContext : IDataInput, IDisposable
    {
        readonly Service service;

        readonly IDoerContext<IDoer> doerctx;

        readonly NpgsqlConnection connection;

        readonly NpgsqlCommand command;

        // can be null
        DbSql sql;

        readonly DbParameters parameters;

        NpgsqlTransaction transact;

        NpgsqlDataReader reader;

        bool multi;

        bool disposed;

        internal DbContext(Service service) : this(service, null)
        {
        }

        internal DbContext(Service service, IDoerContext<IDoer> doerctx)
        {
            this.service = service;
            this.doerctx = doerctx;

            connection = new NpgsqlConnection(service.ConnectionString);
            command = new NpgsqlCommand();
            parameters = new DbParameters(command.Parameters);
            command.Connection = connection;
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

        public DbSql Sql(string str)
        {
            if (sql == null)
            {
                sql = new DbSql(str);
            }
            else
            {
                sql.Clear(); // reset
                sql.Add(str);
            }
            return sql;
        }

        public bool Query1(Action<DbParameters> p = null, bool prepare = true)
        {
            return Query1(sql.ToString(), p, prepare);
        }

        public bool Query1(string cmdtext, Action<DbParameters> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            multi = false;
            command.CommandText = cmdtext;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(parameters);
                if (prepare) command.Prepare();
            }
            reader = command.ExecuteReader();
            return reader.Read();
        }

        public bool Query(Action<DbParameters> p = null, bool prepare = true)
        {
            return Query(sql.ToString(), p, prepare);
        }

        public bool Query(string cmdtext, Action<DbParameters> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            multi = true;
            command.CommandText = cmdtext;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(parameters);
                if (prepare) command.Prepare();
            }
            reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public async Task<bool> QueryAsync(string cmdtext, Action<DbParameters> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            multi = true;
            command.CommandText = cmdtext;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(parameters);
                if (prepare) command.Prepare();
            }
            reader = (NpgsqlDataReader)await command.ExecuteReaderAsync();
            return reader.HasRows;
        }

        public bool Next()
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

        public bool DataSet => multi;

        public bool NextResult()
        {
            ordinal = 0; // reset column ordinal

            if (reader == null)
            {
                return false;
            }
            return reader.NextResult();
        }

        public int Execute(Action<DbParameters> p = null, bool prepare = true)
        {
            return Execute(sql.ToString(), p, prepare);
        }

        public int Execute(string cmdtext, Action<DbParameters> p = null, bool prepare = true)
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
                if (prepare) command.Prepare();
            }
            return command.ExecuteNonQuery();
        }

        public async Task<int> ExecuteAsync(Action<DbParameters> p = null, bool prepare = true)
        {
            return await ExecuteAsync(sql.ToString(), p, prepare);
        }

        public async Task<int> ExecuteAsync(string cmdtext, Action<DbParameters> p = null, bool prepare = true)
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
                if (prepare) command.Prepare();
            }
            return await command.ExecuteNonQueryAsync();
        }

        public object Scalar(Action<DbParameters> p = null, bool prepare = true)
        {
            return Scalar(sql.ToString(), p, prepare);
        }

        public object Scalar(string cmdtext, Action<DbParameters> p = null, bool prepare = true)
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
                if (prepare) command.Prepare();
            }
            return command.ExecuteScalar();
        }

        public async Task<object> ScalarAsync(Action<DbParameters> p = null, bool prepare = true)
        {
            return await ScalarAsync(sql.ToString(), p, prepare);
        }

        public async Task<object> ScalarAsync(string cmdtext, Action<DbParameters> p = null, bool prepare = true)
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
                if (prepare) command.Prepare();
            }
            return await command.ExecuteScalarAsync();
        }

        // TODO
        public async Task<object> CallAsync(string storedproc, Action<DbParameters> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            command.CommandText = storedproc;
            command.CommandType = CommandType.StoredProcedure;
            if (p != null)
            {
                p(parameters);
                if (prepare) command.Prepare();
            }
            return await command.ExecuteScalarAsync();
        }

        //
        // RESULTSET
        //

        public D ToObject<D>(int proj = 0) where D : IData, new()
        {
            D obj = new D();
            obj.ReadData(this, proj);

            // add shard if any
            IShardable sharded = obj as IShardable;
            if (sharded != null)
            {
                sharded.Shard = service.Shard;
            }

            return obj;
        }

        public D[] ToArray<D>(int proj = 0) where D : IData, new()
        {
            return ToList<D>(proj).ToArray();
        }

        public List<D> ToList<D>(int proj = 0) where D : IData, new()
        {
            List<D> lst = new List<D>(32);
            while (Next())
            {
                D obj = new D();
                obj.ReadData(this, proj);

                // add shard if any
                IShardable sharded = obj as IShardable;
                if (sharded != null)
                {
                    sharded.Shard = service.Shard;
                }

                lst.Add(obj);
            }
            return lst;
        }

        // current column ordinal
        int ordinal;

        public bool Get(string name, ref bool v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetBoolean(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetInt16(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetInt32(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetInt64(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetDouble(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetDecimal(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetDateTime(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref NpgsqlPoint v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<NpgsqlPoint>(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetString(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    int len;
                    if ((len = (int)reader.GetBytes(ord, 0, null, 0, 0)) > 0)
                    {
                        byte[] buf = BufferUtility.ByteBuffer(len);
                        reader.GetBytes(ord, 0, buf, 0, len); // read data into the buffer
                        v = new ArraySegment<byte>(buf, 0, len);
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public bool Get(string name, ref Dictionary<string, string> v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, int proj = 0) where D : IData, new()
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParse p = new JsonParse(str);
                    JObj jo = (JObj)p.Parse();
                    v = new D();
                    v.ReadData(jo, proj);

                    // add shard if any
                    IShardable sharded = v as IShardable;
                    if (sharded != null)
                    {
                        sharded.Shard = service.Shard;
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref JObj v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParse p = new JsonParse(str);
                    v = (JObj)p.Parse();
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref JArr v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParse parse = new JsonParse(str);
                    v = (JArr)parse.Parse();
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref IDataInput v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<short[]>(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<int[]>(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<long[]>(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<string[]>(ord);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get<D>(string name, ref D[] v, int proj = 0) where D : IData, new()
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParse parse = new JsonParse(str);
                    JArr ja = (JArr)parse.Parse();
                    int len = ja.Count;
                    v = new D[len];
                    for (int i = 0; i < len; i++)
                    {
                        JObj jo = ja[i];
                        D obj = new D();
                        obj.ReadData(jo, proj);

                        // add shard if any
                        IShardable sharded = obj as IShardable;
                        if (sharded != null)
                        {
                            sharded.Shard = service.Shard;
                        }

                        v[i] = obj;
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }

        public bool Get<D>(string name, ref List<D> v, int proj = 0) where D : IData, new()
        {
            try
            {
                int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParse p = new JsonParse(str);
                    JArr ja = (JArr)p.Parse();
                    int len = ja.Count;
                    v = new List<D>(len + 8);
                    for (int i = 0; i < len; i++)
                    {
                        JObj jo = ja[i];
                        D obj = new D();
                        obj.ReadData(jo, proj);

                        // add shard if any
                        IShardable sharded = obj as IShardable;
                        if (sharded != null)
                        {
                            sharded.Shard = service.Shard;
                        }

                        v.Add(obj);
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }

        //
        // EVENTS
        //

        public void Publish(string name, string shard, int arg, IDataInput inp)
        {
            IContent cont = inp.Dump();
            Publish(name, shard, arg, cont);
            BufferUtility.Return(cont); // back to pool
        }

        public void Publish(string name, string shard, int arg, IData obj, int proj = 0)
        {
            JsonContent cont = new JsonContent(true, true).Put(null, obj, proj);
            Publish(name, shard, arg, cont);
            BufferUtility.Return(cont); // back to pool
        }

        public void Publish<D>(string name, string shard, int arg, D[] arr, int proj = 0) where D : IData
        {
            JsonContent cont = new JsonContent(true, true).Put(null, arr, proj);
            Publish(name, shard, arg, cont);
            BufferUtility.Return(cont); // back to pool
        }

        public void Publish<D>(string name, string shard, int arg, List<D> lst, int proj = 0) where D : IData
        {
            JsonContent cont = new JsonContent(true, true).Put(null, lst, proj);
            Publish(name, shard, arg, cont);
            BufferUtility.Return(cont); // back to pool
        }

        public void Publish(string name, string shard, int arg, IContent content)
        {
            // convert message to byte buffer
            var byteas = new ArraySegment<byte>(content.ByteBuffer, 0, content.Size);
            Execute("INSERT INTO eq (name, shard, arg, body) VALUES (@1, @2, @3, @4)", p =>
            {
                p.Set(name);
                p.Set(shard);
                p.Set(arg);
                p.Set(byteas);
            });
        }

        public void WriteData<R>(IDataOutput<R> o) where R : IDataOutput<R>
        {
            int count = reader.FieldCount;
            for (int i = 0; i < count; i++)
            {
                string name = reader.GetName(i);
                uint oid = reader.GetDataTypeOID(i);

                if (reader.IsDBNull(i))
                {
                    o.PutNull(name);
                    continue;
                }

                if (oid == 1043 || oid == 1042)
                {
                    o.Put(name, reader.GetString(i));
                }
                else if (oid == 790) // money
                {
                    o.Put(name, reader.GetDecimal(i));
                }
            }
        }

        public IContent Dump()
        {
            var cont = new JsonContent(true, true);
            cont.Put(null, this);
            return cont;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                // return to pool
                if (sql != null) BufferUtility.Return(sql);

                // commit ongoing transaction
                if (transact != null && !transact.IsCompleted)
                {
                    transact.Commit();
                }

                reader?.Dispose();
                command.Dispose();
                connection.Dispose();
                // indicate that the instance has been disposed.
                disposed = true;
            }
        }

    }
}