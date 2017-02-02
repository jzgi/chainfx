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
        readonly IHandleContext<IHandle> handlectx;

        readonly WebServiceContext servicectx;

        readonly NpgsqlConnection connection;

        readonly NpgsqlCommand command;

        readonly DbParameters parameters;

        NpgsqlTransaction transact;

        NpgsqlDataReader reader;

        bool multi;

        bool disposed;

        internal DbContext(WebServiceContext servicectx) : this(servicectx, null)
        {
        }

        internal DbContext(WebServiceContext servicectx, IHandleContext<IHandle> handlectx)
        {
            this.servicectx = servicectx;
            this.handlectx = handlectx;

            connection = new NpgsqlConnection(servicectx.ConnectionString);
            command = new NpgsqlCommand();
            parameters = new DbParameters(command.Parameters);
            command.Connection = connection;
        }

        public IsolationLevel Transact
        {
            get { return transact?.IsolationLevel ?? IsolationLevel.Unspecified; }
            set
            {
                if (transact == null && value > IsolationLevel.Unspecified)
                {
                    transact = connection.BeginTransaction(value);
                    command.Transaction = transact;
                }
            }
        }

        public void Begin(IsolationLevel level = IsolationLevel.ReadCommitted)
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

        public bool Query1(DbSql sql, Action<DbParameters> p = null, bool prepare = true)
        {
            bool v = Query1(sql.ToString(), p, prepare);
            BufferUtility.Return(sql);
            return v;
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

        public bool Query(DbSql sql, Action<DbParameters> p = null, bool prepare = true)
        {
            bool v = Query(sql.ToString(), p, prepare);
            BufferUtility.Return(sql);
            return v;
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

        public int Execute(DbSql sql, Action<DbParameters> p = null, bool prepare = true)
        {
            int v = Execute(sql.ToString(), p, prepare);
            BufferUtility.Return(sql);
            return v;
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

        public async Task<int> ExecuteAsync(DbSql sql, Action<DbParameters> p = null, bool prepare = true)
        {
            int v = await ExecuteAsync(sql.ToString(), p, prepare);
            BufferUtility.Return(sql);
            return v;
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

        public object Scalar(DbSql sql, Action<DbParameters> p = null, bool prepare = true)
        {
            object v = Scalar(sql.ToString(), p, prepare);
            BufferUtility.Return(sql);
            return v;
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

        public async Task<object> ScalarAsync(DbSql sql, Action<DbParameters> p = null, bool prepare = true)
        {
            object v = await ScalarAsync(sql.ToString(), p, prepare);
            BufferUtility.Return(sql);
            return v;
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

        public D ToObject<D>(ushort proj = 0) where D : IData, new()
        {
            D obj = new D();
            obj.ReadData(this, proj);

            // add shard if any
            ISharded sharded = obj as ISharded;
            if (sharded != null)
            {
                sharded.Shard = servicectx.shard;
            }

            return obj;
        }

        public D[] ToArray<D>(ushort proj = 0) where D : IData, new()
        {
            return ToList<D>(proj).ToArray();
        }

        public List<D> ToList<D>(ushort proj = 0) where D : IData, new()
        {
            List<D> lst = new List<D>(32);
            while (Next())
            {
                D obj = new D();
                obj.ReadData(this, proj);

                // add shard if any
                ISharded sharded = obj as ISharded;
                if (sharded != null)
                {
                    sharded.Shard = servicectx.shard;
                }

                lst.Add(obj);
            }
            return lst;
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
                    if ((len = (int)reader.GetBytes(ord, 0, null, 0, 0)) > 0)
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

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            int ord = name == null ? ordinal++ : reader.GetOrdinal(name);
            try
            {
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
            catch
            {
            }

            return false;
        }

        public bool Get<D>(string name, ref D v, ushort proj = 0) where D : IData, new()
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
                ISharded sharded = v as ISharded;
                if (sharded != null)
                {
                    sharded.Shard = servicectx.shard;
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
                v = (JObj)p.Parse();
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
                JsonParse parse = new JsonParse(str);
                v = (JArr)parse.Parse();
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

        public bool Get<D>(string name, ref D[] v, ushort proj = 0) where D : IData, new()
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
                    ISharded sharded = obj as ISharded;
                    if (sharded != null)
                    {
                        sharded.Shard = servicectx.shard;
                    }

                    v[i] = obj;
                }
                return true;
            }
            return false;
        }

        public bool Get<D>(string name, ref List<D> v, ushort proj = 0) where D : IData, new()
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
                    ISharded sharded = obj as ISharded;
                    if (sharded != null)
                    {
                        sharded.Shard = servicectx.shard;
                    }

                    v.Add(obj);
                }
                return true;
            }
            return false;
        }

        //
        // MESSAGING
        //

        public void Event(string name, string shard, IData dat, byte bits = 0)
        {
            JsonContent cont = new JsonContent(true, true);
            cont.Put(null, dat, bits);
            Event(name, shard, cont);
        }

        public void Event<D>(string name, string shard, D[] dats, byte bits = 0) where D : IData
        {
            JsonContent cont = new JsonContent();
            cont.Put(null, dats, bits);
            Event(name, shard, cont);
        }

        public void Event(string name, string shard, IDataInput inp)
        {
        }

        public void Event(string name, string shard, ArraySegment<byte> bytesseg)
        {
        }

        public void Event<C>(string name, string shard, C content) where C : IContent
        {
            // convert message to byte buffer
            ArraySegment<byte> byteseg = new ArraySegment<byte>(content.ByteBuffer, 0, content.Size);
            Execute("INSERT INTO eq (name, shard, body) VALUES (@1, @2, @3)", p =>
            {
                p.Set(name);
                p.Set(shard);
                p.Set(byteseg);
            });
            BufferUtility.Return(content); // back to pool
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
                };

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

        public C Dump<C>() where C : IContent, IDataOutput<C>, new()
        {
            C cont = new C();
            cont.Put(null, this);
            return cont;
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

        public bool Get(string name, ref IDataInput v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref Dictionary<string, string> v)
        {
            throw new NotImplementedException();
        }

        struct ColumnDesc
        {
            internal string name;

            internal Type type;

            internal uint oid;
        }
    }
}