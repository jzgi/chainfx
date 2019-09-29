using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Db
{
    /// <summary>
    /// An environment for database operations. It provides strong-typed reads/writes and lightweight O/R mapping.
    /// </summary>
    public sealed class DbContext : ISource, IParameterSet, IDisposable
    {
        static readonly string[] PARAMS =
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16",
            "17", "18", "19", "20", "21", "22", "23", "24", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32"
        };

        static readonly string[] INPARAMS =
        {
            "v1", "v2", "v3", "v4", "v5", "v6", "v7", "v8", "v9", "v10", "v11", "v12", "v13", "v14", "v15", "v16",
            "v17", "v18", "v19", "v20", "v21", "v22", "v23", "v24", "v15", "v16", "v17", "v18", "v19", "v20", "v21", "v22", "v23", "v24", "v25", "v26", "v27", "v28", "v29", "v30", "v31", "v32"
        };

        readonly DbSource source;

        readonly NpgsqlConnection connection;

        readonly NpgsqlCommand command;

        // generator of sql string, can be null
        DbSql _sql;

        NpgsqlTransaction transact;

        NpgsqlDataReader reader;

        bool multiple;

        bool disposing;

        // current parameter index
        int paramidx;

        internal DbContext(DbSource src)
        {
            source = src;

            connection = new NpgsqlConnection(src.ConnectionString);
            command = new NpgsqlCommand
            {
                Connection = connection
            };
        }

        public DbSource Source => source;

        public bool IsMultiple => multiple;

        void Clear()
        {
            // reader reset
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }

            ordinal = 0;
            // command parameter reset
            command.Parameters.Clear();
            paramidx = 0;
        }

        public void Dispose()
        {
            if (!disposing)
            {
                // indicate disposing the instance 
                disposing = true;
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

        public bool IsDataSet => multiple;

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
            if (_sql == null)
            {
                _sql = new DbSql(str);
            }
            else
            {
                _sql.Clear(); // reset
                _sql.Add(str);
            }

            return _sql;
        }

        public bool Query(Action<IParameterSet> p = null, bool prepare = true)
        {
            return Query(_sql.ToString(), p, prepare);
        }

        public bool Query(string sql, Action<IParameterSet> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            multiple = false;
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

        public async Task<bool> QueryAsync(Action<IParameterSet> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            multiple = false;
            command.CommandText = _sql.ToString();
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }

            reader = (NpgsqlDataReader) await command.ExecuteReaderAsync();
            return reader.Read();
        }

        public async Task<bool> QueryAsync(string sql, Action<IParameterSet> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            multiple = false;
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

        public D Query<D>(Action<IParameterSet> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (Query(p, prepare))
            {
                return ToObject<D>(proj);
            }

            return default;
        }

        public D Query<D>(string sql, Action<IParameterSet> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (Query(sql, p, prepare))
            {
                return ToObject<D>(proj);
            }

            return default;
        }

        public async Task<D> QueryAsync<D>(Action<IParameterSet> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (await QueryAsync(p, prepare))
            {
                return ToObject<D>(proj);
            }

            return default;
        }

        public async Task<D> QueryAsync<D>(string sql, Action<IParameterSet> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (await QueryAsync(sql, p, prepare))
            {
                return ToObject<D>(proj);
            }

            return default;
        }

        public bool QueryAll(Action<IParameterSet> p = null, bool prepare = true)
        {
            return QueryAll(_sql.ToString(), p, prepare);
        }

        public bool QueryAll(string sql, Action<IParameterSet> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            multiple = true;
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

        public async Task<bool> QueryAllAsync(Action<IParameterSet> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            multiple = true;
            command.CommandText = _sql.ToString();
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }

            reader = (NpgsqlDataReader) await command.ExecuteReaderAsync();
            return reader.HasRows;
        }

        public async Task<bool> QueryAllAsync(string sql, Action<IParameterSet> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            multiple = true;
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

        public D[] QueryAll<D>(Action<IParameterSet> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (QueryAll(p, prepare))
            {
                return ToArray<D>(proj);
            }

            return null;
        }

        public D[] QueryAll<D>(string sql, Action<IParameterSet> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (QueryAll(sql, p, prepare))
            {
                return ToArray<D>(proj);
            }

            return null;
        }

        public async Task<D[]> QueryAllAsync<D>(Action<IParameterSet> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (await QueryAllAsync(p, prepare))
            {
                return ToArray<D>(proj);
            }

            return null;
        }

        public async Task<D[]> QueryAllAsync<D>(string sql, Action<IParameterSet> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (await QueryAllAsync(sql, p, prepare))
            {
                return ToArray<D>(proj);
            }

            return null;
        }

        public Map<K, D> QueryAll<K, D>(Action<IParameterSet> p = null, byte proj = 0x0f, Func<D, K> keyer = null, bool prepare = true) where D : IData, new()
        {
            if (QueryAll(p, prepare))
            {
                return ToMap(proj, keyer);
            }

            return null;
        }

        public Map<K, D> QueryAll<K, D>(string sql, Action<IParameterSet> p = null, byte proj = 0x0f, Func<D, K> keyer = null, bool prepare = true) where D : IData, new()
        {
            if (QueryAll(sql, p, prepare))
            {
                return ToMap(proj, keyer);
            }

            return null;
        }

        public async Task<Map<K, D>> QueryAllAsync<K, D>(Action<IParameterSet> p = null, byte proj = 0x0f, Func<D, K> keyer = null, bool prepare = true) where D : IData, new()
        {
            if (await QueryAllAsync(p, prepare))
            {
                return ToMap(proj, keyer);
            }

            return null;
        }

        public async Task<Map<K, D>> QueryAllAsync<K, D>(string sql, Action<IParameterSet> p = null, byte proj = 0x0f, Func<D, K> keyer = null, bool prepare = true) where D : IData, new()
        {
            if (await QueryAllAsync(sql, p, prepare))
            {
                return ToMap(proj, keyer);
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

        public int Execute(Action<IParameterSet> p = null, bool prepare = true)
        {
            return Execute(_sql.ToString(), p, prepare);
        }

        public int Execute(string sql, Action<IParameterSet> p = null, bool prepare = true)
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

        public async Task<int> ExecuteAsync(Action<IParameterSet> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            command.CommandText = _sql.ToString();
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> ExecuteAsync(string sql, Action<IParameterSet> p = null, bool prepare = true)
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

        public object Scalar(Action<IParameterSet> p = null, bool prepare = true)
        {
            return Scalar(_sql.ToString(), p, prepare);
        }

        public object Scalar(string sql, Action<IParameterSet> p = null, bool prepare = true)
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

        public async Task<object> ScalarAsync(Action<IParameterSet> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            command.CommandText = _sql.ToString();
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }

            return await command.ExecuteScalarAsync();
        }

        public async Task<object> ScalarAsync(string sql, Action<IParameterSet> p = null, bool prepare = true)
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
            return obj;
        }

        public D[] ToArray<D>(byte proj = 0x0f) where D : IData, new()
        {
            ValueList<D> lst = new ValueList<D>(32);
            while (Next())
            {
                D obj = new D();
                obj.Read(this, proj);
                lst.Add(obj);
            }

            return lst.ToArray();
        }

        public Map<K, D> ToMap<K, D>(byte proj = 0x0f, Func<D, K> keyer = null) where D : IData, new()
        {
            var map = new Map<K, D>(64);
            while (Next())
            {
                D obj = new D();
                obj.Read(this, proj);
                K key;
                if (keyer != null)
                {
                    key = keyer(obj);
                }
                else if (obj is IKeyable<K> keyable)
                {
                    key = keyable.Key;
                }
                else
                {
                    throw new FrameworkException("neither keyer nor IKeyable<D>");
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

        public bool Get(string name, ref char v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetChar(ord);
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

        public bool Get(string name, ref uint v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<uint>(ord);
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        public bool Get(string name, ref float v)
        {
            throw new NotImplementedException();
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

        public bool Get(string name, ref Guid v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetGuid(ord);
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

        public bool Get(string name, ref bool[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref float[] v)
        {
            throw new NotImplementedException();
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

        public bool Get(string name, ref char[] v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<char[]>(ord);
                    return true;
                }
            }
            catch
            {
            }

            return false;
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

        public bool Get(string name, ref uint[] v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<uint[]>(ord);
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        public bool Get(string name, ref double[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref decimal[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref DateTime[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref Guid[] v)
        {
            throw new NotImplementedException();
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

        public bool Let(out bool v)
        {
            v = false;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetBoolean(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public char Let(out char v)
        {
            v = '\0';
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetChar(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public short Let(out short v)
        {
            v = 0;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetInt16(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public uint Let(out uint v)
        {
            v = 0;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<uint>(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public int Let(out int v)
        {
            v = 0;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetInt32(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public long Let(out long v)
        {
            v = 0;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetInt64(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public double Let(out double v)
        {
            v = 0;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetDouble(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public decimal Let(out decimal v)
        {
            v = 0;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetDecimal(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public DateTime Let(out DateTime v)
        {
            v = default;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetDateTime(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public string Let(out string v)
        {
            v = null;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetString(ord);
                }
            }
            catch
            {
            }

            return v;
        }


        public Guid Let(out Guid v)
        {
            v = default;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetGuid(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public byte[] Let(out byte[] v)
        {
            v = default;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    int len;
                    if ((len = (int) reader.GetBytes(ord, 0, null, 0, 0)) > 0)
                    {
                        var buf = new byte[len];
                        reader.GetBytes(ord, 0, buf, 0, len); // read data into the buffer
                        v = buf;
                    }
                }
            }
            catch
            {
            }

            return v;
        }

        public char[] Let(out char[] v)
        {
            v = null;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<char[]>(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public short[] Let(out short[] v)
        {
            v = null;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<short[]>(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public int[] Let(out int[] v)
        {
            v = null;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<int[]>(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public void Let(out long[] v)
        {
            v = null;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<long[]>(ord);
                }
            }
            catch
            {
            }
        }

        public string[] Let(out string[] v)
        {
            v = null;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<string[]>(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public void Let(out JObj v)
        {
            throw new NotImplementedException();
        }

        public void Let(out JArr v)
        {
            throw new NotImplementedException();
        }

        public D Let<D>(out D v, byte proj = 0x0f) where D : IData, new()
        {
            v = default;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    var p = new JsonParser(str);
                    var jo = (JObj) p.Parse();
                    v = new D();
                    v.Read(jo, proj);
                }
            }
            catch
            {
            }

            return v;
        }

        public D[] Let<D>(out D[] v, byte proj = 0x0f) where D : IData, new()
        {
            v = null;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    var parser = new JsonParser(str);
                    var ja = (JArr) parser.Parse();
                    int len = ja.Count;
                    v = new D[len];
                    for (int i = 0; i < len; i++)
                    {
                        JObj jo = ja[i];
                        D obj = new D();
                        obj.Read(jo, proj);
                        v[i] = obj;
                    }
                }
            }
            catch
            {
            }

            return v;
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
            var cnt = new JsonContent(8192);
            int fc = reader.FieldCount;
            while (reader.Read())
            {
                cnt.ARR_();
                for (int i = 0; i < fc; i++)
                {
                    cnt.OBJ_();
                    var typ = reader.GetFieldType(i);
                    if (typ == typeof(short))
                    {
                        cnt.Put(reader.GetName(i), reader.GetInt16(i));
                    }
                    else if (typ == typeof(int))
                    {
                        cnt.Put(reader.GetName(i), reader.GetInt32(i));
                    }
                    else if (typ == typeof(string))
                    {
                        cnt.Put(reader.GetName(i), reader.GetString(i));
                    }

                    cnt._OBJ();
                }

                cnt._ARR();
            }
            return cnt;
        }


        //
        // PARAMETERS

        public void PutNull(string name)
        {
            command.Parameters.AddWithValue(name, DBNull.Value);
        }

        public void Put(string name, JNumber v)
        {
            command.Parameters.Add(new NpgsqlParameter<decimal>(name, NpgsqlDbType.Numeric)
            {
                TypedValue = v.Decimal
            });
        }

        public void Put(string name, bool v)
        {
            command.Parameters.Add(new NpgsqlParameter<bool>(name, NpgsqlDbType.Boolean)
            {
                TypedValue = v
            });
        }

        public void Put(string name, char v)
        {
            command.Parameters.Add(new NpgsqlParameter<char>(name, NpgsqlDbType.Char)
            {
                TypedValue = v
            });
        }

        public void Put(string name, byte v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, short v)
        {
            command.Parameters.Add(new NpgsqlParameter<short>(name, NpgsqlDbType.Smallint)
            {
                TypedValue = v
            });
        }

        public void Put(string name, int v)
        {
            command.Parameters.Add(new NpgsqlParameter<int>(name, NpgsqlDbType.Integer)
            {
                TypedValue = v
            });
        }

        public void Put(string name, long v)
        {
            command.Parameters.Add(new NpgsqlParameter<long>(name, NpgsqlDbType.Bigint)
            {
                TypedValue = v
            });
        }

        public void Put(string name, uint v)
        {
            command.Parameters.Add(new NpgsqlParameter<uint>(name, NpgsqlDbType.Oid)
            {
                TypedValue = v
            });
        }

        public void Put(string name, float v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, double v)
        {
            command.Parameters.Add(new NpgsqlParameter<double>(name, NpgsqlDbType.Double)
            {
                TypedValue = v
            });
        }

        public void Put(string name, decimal v)
        {
            command.Parameters.Add(new NpgsqlParameter<decimal>(name, NpgsqlDbType.Money)
            {
                TypedValue = v
            });
        }

        public void Put(string name, DateTime v)
        {
            bool date = v.Hour == 0 && v.Minute == 0 && v.Second == 0 && v.Millisecond == 0;
            command.Parameters.Add(new NpgsqlParameter<DateTime>(name, date ? NpgsqlDbType.Date : NpgsqlDbType.Timestamp)
            {
                TypedValue = v
            });
        }

        public void Put(string name, Guid v)
        {
            command.Parameters.Add(new NpgsqlParameter<Guid>(name, NpgsqlDbType.Uuid)
            {
                TypedValue = v
            });
        }

        public void Put(string name, string v)
        {
            int len = v?.Length ?? 0;
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Varchar, len)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, bool[] v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, char[] v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, ArraySegment<byte> v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea, v.Count)
            {
                Value = (v.Array != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, byte[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea, v?.Length ?? 0)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, short[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Smallint)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, int[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, long[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Bigint)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, float[] v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, double[] v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, decimal[] v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, DateTime[] v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, Guid[] v)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, string[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void Put(string name, JObj v)
        {
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

        public void PutFromSource(ISource s)
        {
            throw new NotImplementedException();
        }
        //
        // positional
        //

        public IParameterSet SetNull()
        {
            PutNull(PARAMS[paramidx++]);
            return this;
        }

        public IParameterSet Set(bool v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(char v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(byte v)
        {
            throw new NotImplementedException();
        }

        public IParameterSet Set(short v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(int v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(long v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(uint v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(float v)
        {
            throw new NotImplementedException();
        }

        public IParameterSet Set(double v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(decimal v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(JNumber v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(DateTime v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(Guid v)
        {
            throw new NotImplementedException();
        }

        public IParameterSet Set(string v)
        {
            if (v == string.Empty)
            {
                v = null;
            }

            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(bool[] v)
        {
            throw new NotImplementedException();
        }

        public IParameterSet Set(char[] v)
        {
            throw new NotImplementedException();
        }

        public IParameterSet Set(byte[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(ArraySegment<byte> v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(short[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(int[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(long[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(uint[] v)
        {
            throw new NotImplementedException();
        }

        public IParameterSet Set(float[] v)
        {
            throw new NotImplementedException();
        }

        public IParameterSet Set(double[] v)
        {
            throw new NotImplementedException();
        }

        public IParameterSet Set(DateTime[] v)
        {
            throw new NotImplementedException();
        }

        public IParameterSet Set(Guid[] v)
        {
            throw new NotImplementedException();
        }

        public IParameterSet Set(string[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameterSet Set(JObj v)
        {
            throw new NotImplementedException();
        }

        public IParameterSet Set(JArr v)
        {
            throw new NotImplementedException();
        }

        public IParameterSet Set(IData v, byte proj = 0x0f)
        {
            Put(PARAMS[paramidx++], v, proj);
            return this;
        }

        public IParameterSet Set<D>(D[] v, byte proj = 0x0f) where D : IData
        {
            Put(PARAMS[paramidx++], v, proj);
            return this;
        }

        public IParameterSet SetForIn(string[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                Put(INPARAMS[i], v[i]);
            }

            return this;
        }

        public IParameterSet SetForIn(short[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                Put(INPARAMS[i], v[i]);
            }

            return this;
        }

        public IParameterSet SetForIn(int[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                Put(INPARAMS[i], v[i]);
            }

            return this;
        }

        public IParameterSet SetForIn(long[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                Put(INPARAMS[i], v[i]);
            }

            return this;
        }
    }
}