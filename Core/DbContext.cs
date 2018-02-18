using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    /// <summary>
    /// An environment for database operations based on current service.
    /// </summary>
    public class DbContext : ISource, IParams, IDisposable
    {
        static readonly string[] PARAMS =
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24"
        };

        readonly Service service;

        readonly NpgsqlConnection connection;

        readonly NpgsqlCommand command;

        // generator of sql string, can be null
        DbSql sql;

        NpgsqlTransaction transact;

        NpgsqlDataReader reader;

        bool multi;

        bool disposing;

        // current parameter index
        int index;

        internal DbContext(Service service)
        {
            this.service = service;
            connection = new NpgsqlConnection(service.ConnectionString);
            command = new NpgsqlCommand
            {
                Connection = connection
            };
        }

        void Clear()
        {
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
            ordinal = 0;
            command.Parameters.Clear();
            index = 0;
        }

        public void Dispose()
        {
            if (!disposing)
            {
                // indicate disposing the instance 
                disposing = true;
                // return to chars pool
                if (sql != null) BufferUtility.Return(sql);
                // commit ongoing transaction
                if (transact != null && !transact.IsCompleted)
                {
                    Clear();
                    transact.Commit();
                }
                reader?.Close();
                command.Transaction = null;
                connection.Close();
            }
        }

        public void Begin(IsolationLevel level = IsolationLevel.ReadCommitted)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
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
            if (transact != null && !transact.IsCompleted)
            {
                // indicate disposing the instance 
                Clear();
                transact.Rollback();
                command.Transaction = null;
                transact = null;
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

        public bool Next()
        {
            ordinal = 0; // reset column ordinal

            if (reader == null)
            {
                return false;
            }
            return reader.Read();
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

        public bool Query1(Action<IParams> p = null, bool prepare = true)
        {
            return Query1(sql.ToString(), p, prepare);
        }

        public bool Query1(string sql, Action<IParams> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            multi = false;
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            reader = command.ExecuteReader();
            return reader.Read();
        }

        public async Task<bool> Query1Async(Action<IParams> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            multi = false;
            command.CommandText = sql.ToString();
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            reader = (NpgsqlDataReader) await command.ExecuteReaderAsync();
            return reader.Read();
        }

        public async Task<bool> Query1Async(string sql, Action<IParams> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            multi = false;
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            reader = (NpgsqlDataReader) await command.ExecuteReaderAsync();
            return reader.Read();
        }

        public D Query1<D>(Action<IParams> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (Query1(p, prepare))
            {
                return ToObject<D>(proj);
            }
            return default;
        }

        public D Query1<D>(string sql, Action<IParams> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (Query1(sql, p, prepare))
            {
                return ToObject<D>(proj);
            }
            return default;
        }

        public async Task<D> Query1Async<D>(Action<IParams> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (await Query1Async(p, prepare))
            {
                return ToObject<D>(proj);
            }
            return default;
        }

        public async Task<D> Query1Async<D>(string sql, Action<IParams> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (await Query1Async(sql, p, prepare))
            {
                return ToObject<D>(proj);
            }
            return default;
        }

        public bool Query(Action<IParams> p = null, bool prepare = true)
        {
            return Query(sql.ToString(), p, prepare);
        }

        public bool Query(string sql, Action<IParams> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            multi = true;
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public async Task<bool> QueryAsync(Action<IParams> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            multi = true;
            command.CommandText = sql.ToString();
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            reader = (NpgsqlDataReader) await command.ExecuteReaderAsync();
            return reader.HasRows;
        }

        public async Task<bool> QueryAsync(string sql, Action<IParams> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            multi = true;
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            reader = (NpgsqlDataReader) await command.ExecuteReaderAsync();
            return reader.HasRows;
        }

        public D[] Query<D>(Action<IParams> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (Query(p, prepare))
            {
                return ToArray<D>(proj);
            }
            return null;
        }

        public D[] Query<D>(string sql, Action<IParams> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (Query(sql, p, prepare))
            {
                return ToArray<D>(proj);
            }
            return null;
        }

        public async Task<D[]> QueryAsync<D>(Action<IParams> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (await QueryAsync(p, prepare))
            {
                return ToArray<D>(proj);
            }
            return null;
        }

        public async Task<D[]> QueryAsync<D>(string sql, Action<IParams> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (await QueryAsync(sql, p, prepare))
            {
                return ToArray<D>(proj);
            }
            return null;
        }

        public Map<K, D> Query<K, D>(Action<IParams> p = null, byte proj = 0x0f, Func<D, K> keyer = null, Predicate<K> toper = null, bool prepare = true) where D : IData, new()
        {
            if (Query(p, prepare))
            {
                return ToMap(proj, keyer, toper);
            }
            return null;
        }

        public Map<K, D> Query<K, D>(string sql, Action<IParams> p = null, byte proj = 0x0f, Func<D, K> keyer = null, Predicate<K> toper = null, bool prepare = true) where D : IData, new()
        {
            if (Query(sql, p, prepare))
            {
                return ToMap(proj, keyer, toper);
            }
            return null;
        }

        public async Task<Map<K, D>> QueryAsync<K, D>(Action<IParams> p = null, byte proj = 0x0f, Func<D, K> keyer = null, Predicate<K> toper = null, bool prepare = true) where D : IData, new()
        {
            if (await QueryAsync(p, prepare))
            {
                return ToMap(proj, keyer, toper);
            }
            return null;
        }

        public async Task<Map<K, D>> QueryAsync<K, D>(string sql, Action<IParams> p = null, byte proj = 0x0f, Func<D, K> keyer = null, Predicate<K> toper = null, bool prepare = true) where D : IData, new()
        {
            if (await QueryAsync(sql, p, prepare))
            {
                return ToMap(proj, keyer, toper);
            }
            return null;
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

        public int Execute(Action<IParams> p = null, bool prepare = true)
        {
            return Execute(sql.ToString(), p, prepare);
        }

        public int Execute(string sql, Action<IParams> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            return command.ExecuteNonQuery();
        }

        public async Task<int> ExecuteAsync(Action<IParams> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            command.CommandText = sql.ToString();
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> ExecuteAsync(string sql, Action<IParams> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            return await command.ExecuteNonQueryAsync();
        }

        public object Scalar(Action<IParams> p = null, bool prepare = true)
        {
            return Scalar(sql.ToString(), p, prepare);
        }

        public object Scalar(string sql, Action<IParams> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            object res = command.ExecuteScalar();
            return res == DBNull.Value ? null : res;
        }

        public async Task<object> ScalarAsync(Action<IParams> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            command.CommandText = sql.ToString();
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            return await command.ExecuteScalarAsync();
        }

        public async Task<object> ScalarAsync(string sql, Action<IParams> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Clear();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            return await command.ExecuteScalarAsync();
        }

        //
        // RESULTSET
        //

        public D ToObject<D>(byte proj = 0x0f) where D : IData, new()
        {
            D obj = new D();
            obj.Read(this, proj);
            // add shard if any
            if (obj is IShardable sharded)
            {
                sharded.Shard = service.Shard;
            }
            return obj;
        }

        public D[] ToArray<D>(byte proj = 0x0f) where D : IData, new()
        {
            Roll<D> roll = new Roll<D>(32);
            while (Next())
            {
                D obj = new D();
                obj.Read(this, proj);
                // add shard if any
                if (obj is IShardable sharded)
                {
                    sharded.Shard = service.Shard;
                }
                roll.Add(obj);
            }
            return roll.ToArray();
        }

        public Map<K, D> ToMap<K, D>(byte proj = 0x0f, Func<D, K> keyer = null, Predicate<K> toper = null) where D : IData, new()
        {
            Map<K, D> map = new Map<K, D>(64, toper);
            while (Next())
            {
                D obj = new D();
                obj.Read(this, proj);
                // add shard name if any
                if (obj is IShardable sharded)
                {
                    sharded.Shard = service.Shard;
                }
                K key;
                if (keyer != null)
                {
                    key = keyer(obj);
                }
                else if (obj is IMappable<K> mappable)
                {
                    key = mappable.Key;
                }
                else
                {
                    throw new ServiceException("neither keyer nor IMappable<D>");
                }
                map.Add(key, obj);
            }
            return map;
        }

        // current column ordinal
        int ordinal;

        public bool Get(string name, ref bool v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetBoolean(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref short v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetInt16(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref int v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetInt32(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref long v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetInt64(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref double v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetDouble(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetDecimal(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetDateTime(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref string v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetString(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    int len;
                    if ((len = (int) reader.GetBytes(ord, 0, null, 0, 0)) > 0)
                    {
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
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    int len;
                    if ((len = (int) reader.GetBytes(ord, 0, null, 0, 0)) > 0)
                    {
                        byte[] buf = new byte[len];
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

        public bool Get(string name, ref Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, byte proj = 0x0f) where D : IData, new()
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParser p = new JsonParser(str);
                    JObj jo = (JObj) p.Parse();
                    v = new D();
                    v.Read(jo, proj);
                    // add shard if any
                    if (v is IShardable sharded)
                    {
                        sharded.Shard = service.Shard;
                    }
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref JObj v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParser p = new JsonParser(str);
                    v = (JObj) p.Parse();
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref JArr v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParser parser = new JsonParser(str);
                    v = (JArr) parser.Parse();
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref ISource v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<short[]>(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<int[]>(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<long[]>(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<string[]>(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get<D>(string name, ref D[] v, byte proj = 0x0f) where D : IData, new()
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParser parser = new JsonParser(str);
                    JArr ja = (JArr) parser.Parse();
                    int len = ja.Count;
                    v = new D[len];
                    for (int i = 0; i < len; i++)
                    {
                        JObj jo = ja[i];
                        D obj = new D();
                        obj.Read(jo, proj);

                        // add shard if any
                        if (obj is IShardable sharded)
                        {
                            sharded.Shard = service.Shard;
                        }

                        v[i] = obj;
                    }
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }


        //
        // LET
        //

        public ISource Let(out bool v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetBoolean(ord);
                    return this;
                }
            }
            catch
            {
            }
            v = false;
            return this;
        }

        public ISource Let(out short v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetInt16(ord);
                    return this;
                }
            }
            catch
            {
            }
            v = 0;
            return this;
        }

        public ISource Let(out int v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetInt32(ord);
                    return this;
                }
            }
            catch
            {
            }
            v = 0;
            return this;
        }

        public ISource Let(out long v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetInt64(ord);
                    return this;
                }
            }
            catch
            {
            }
            v = 0;
            return this;
        }

        public ISource Let(out double v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetDouble(ord);
                    return this;
                }
            }
            catch
            {
            }

            v = 0;
            return this;
        }

        public ISource Let(out decimal v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetDecimal(ord);
                    return this;
                }
            }
            catch
            {
            }
            v = 0;
            return this;
        }

        public ISource Let(out DateTime v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetDateTime(ord);
                    return this;
                }
            }
            catch
            {
            }
            v = default;
            return this;
        }

        public ISource Let(out string v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetString(ord);
                    return this;
                }
            }
            catch
            {
            }
            v = null;
            return this;
        }

        public ISource Let(out ArraySegment<byte> v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    int len;
                    if ((len = (int) reader.GetBytes(ord, 0, null, 0, 0)) > 0)
                    {
                        byte[] buf = new byte[len];
                        reader.GetBytes(ord, 0, buf, 0, len); // read data into the buffer
                        v = new ArraySegment<byte>(buf, 0, len);
                        return this;
                    }
                }
            }
            catch
            {
            }
            v = default;
            return this;
        }

        public ISource Let(out short[] v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<short[]>(ord);
                    return this;
                }
            }
            catch
            {
            }
            v = null;
            return this;
        }

        public ISource Let(out int[] v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<int[]>(ord);
                    return this;
                }
            }
            catch
            {
            }
            v = null;
            return this;
        }

        public ISource Let(out long[] v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<long[]>(ord);
                    return this;
                }
            }
            catch
            {
            }
            v = null;
            return this;
        }

        public ISource Let(out string[] v)
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<string[]>(ord);
                    return this;
                }
            }
            catch
            {
            }
            v = null;
            return this;
        }

        public ISource Let(out JObj v)
        {
            throw new NotImplementedException();
        }

        public ISource Let(out JArr v)
        {
            throw new NotImplementedException();
        }

        public ISource Let<D>(out D v, byte proj = 0x0f) where D : IData, new()
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParser p = new JsonParser(str);
                    JObj jo = (JObj) p.Parse();
                    v = new D();
                    v.Read(jo, proj);
                    // add shard if any
                    if (v is IShardable sharded)
                    {
                        sharded.Shard = service.Shard;
                    }
                    return this;
                }
            }
            catch
            {
            }
            v = default;
            return this;
        }

        public ISource Let<D>(out D[] v, byte proj = 0x0f) where D : IData, new()
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParser parser = new JsonParser(str);
                    JArr ja = (JArr) parser.Parse();
                    int len = ja.Count;
                    v = new D[len];
                    for (int i = 0; i < len; i++)
                    {
                        JObj jo = ja[i];
                        D obj = new D();
                        obj.Read(jo, proj);

                        // add shard if any
                        if (obj is IShardable sharded)
                        {
                            sharded.Shard = service.Shard;
                        }
                        v[i] = obj;
                    }
                    return this;
                }
            }
            catch
            {
            }
            v = null;
            return this;
        }

        public void Write<C>(C cnt) where C : IContent, ISink
        {
            int fc = reader.FieldCount;
            for (int i = 0; i < fc; i++)
            {
                string name = reader.GetName(i);
                if (reader.IsDBNull(i))
                {
                    cnt.PutNull(name);
                    continue;
                }
                var typ = reader.GetFieldType(i);
                if (typ == typeof(bool))
                {
                    cnt.Put(name, reader.GetBoolean(i));
                }
                else if (typ == typeof(short))
                {
                    cnt.Put(name, reader.GetInt16(i));
                }
                else if (typ == typeof(int))
                {
                    cnt.Put(name, reader.GetInt32(i));
                }
                else if (typ == typeof(long))
                {
                    cnt.Put(name, reader.GetInt64(i));
                }
                else if (typ == typeof(string))
                {
                    cnt.Put(name, reader.GetString(i));
                }
                else if (typ == typeof(decimal))
                {
                    cnt.Put(name, reader.GetDecimal(i));
                }
                else if (typ == typeof(double))
                {
                    cnt.Put(name, reader.GetDouble(i));
                }
                else if (typ == typeof(DateTime))
                {
                    cnt.Put(name, reader.GetDateTime(i));
                }
            }
        }

        public IContent Dump()
        {
            var cnt = new FlowContent(512 * 1024);
            cnt.PutFrom(this);
            return cnt;
        }


        //
        // PARAMETERS

        public void PutNull(string name)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.AddWithValue(name, DBNull.Value);
        }

        public void Put(string name, JNumber v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Numeric)
            {
                Value = v.Decimal
            });
        }

        public void Put(string name, bool v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Boolean)
            {
                Value = v
            });
        }

        public void Put(string name, short v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Smallint)
            {
                Value = v
            });
        }

        public void Put(string name, int v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = v
            });
        }

        public void Put(string name, long v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Bigint)
            {
                Value = v
            });
        }

        public void Put(string name, double v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Double)
            {
                Value = v
            });
        }

        public void Put(string name, decimal v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = v
            });
        }

        public void Put(string name, DateTime v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            bool date = v.Hour == 0 && v.Minute == 0 && v.Second == 0 && v.Millisecond == 0;
            command.Parameters.Add(new NpgsqlParameter(name, date ? NpgsqlDbType.Date : NpgsqlDbType.Timestamp)
            {
                Value = v
            });
        }

        public void Put(string name, string v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            int len = v?.Length ?? 0;
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Varchar, len)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, ArraySegment<byte> v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea, v.Count)
            {
                Value = (v.Array != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, short[] v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Smallint)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, int[] v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, long[] v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Bigint)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, string[] v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, JObj v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            if (v == null)
            {
                command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = DBNull.Value
                });
            }
            else
            {
                command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = v.ToString()
                });
            }
        }

        public void Put(string name, JArr v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            if (v == null)
            {
                command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = DBNull.Value
                });
            }
            else
            {
                command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = v.ToString()
                });
            }
        }

        public void Put(string name, IData v, byte proj = 0x0f)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            if (v == null)
            {
                command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb) {Value = DBNull.Value});
            }
            else
            {
                command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = DataUtility.ToString(v, proj)
                });
            }
        }

        public void Put<D>(string name, D[] v, byte proj = 0x0f) where D : IData
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            if (v == null)
            {
                command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = DBNull.Value
                });
            }
            else
            {
                command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = DataUtility.ToString(v, proj)
                });
            }
        }

        public void PutFrom(ISource s)
        {
            throw new NotImplementedException();
        }
        //
        // positional
        //

        public IParams SetNull()
        {
            PutNull(null);
            return this;
        }

        public IParams Set(bool v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(short v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(int v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(long v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(double v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(decimal v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(JNumber v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(DateTime v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(string v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(ArraySegment<byte> v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(short[] v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(int[] v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(long[] v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(string[] v)
        {
            Put(null, v);
            return this;
        }

        public IParams Set(JObj v)
        {
            throw new NotImplementedException();
        }

        public IParams Set(JArr v)
        {
            throw new NotImplementedException();
        }

        public IParams Set(IData v, byte proj = 0x0f)
        {
            Put(null, v, proj);
            return this;
        }

        public IParams Set<D>(D[] v, byte proj = 0x0f) where D : IData
        {
            Put(null, v, proj);
            return this;
        }
    }
}