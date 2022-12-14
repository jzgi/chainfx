using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace ChainFx.Fabric
{
    /// <summary>
    /// The working environment for a series of database operations. It provides strong-typed reads/writes and lightweight O/R mapping.
    /// </summary>
    public class DbContext : ISource, IParameters, IDisposable
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

        readonly NpgsqlConnection connection;

        readonly NpgsqlCommand command;

        // builder of sql string, can be null
        DbSql builder;

        NpgsqlTransaction transact;

        NpgsqlDataReader reader;

        bool multiple;

        bool disposing;

        // current parameter index
        int paramidx;

        internal DbContext()
        {
            var dbsource = Nodality.DbSource;
            connection = new NpgsqlConnection(dbsource.ConnectionString);
            command = new NpgsqlCommand
            {
                Connection = connection
            };
        }

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

            checksum = 0;
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
                command.Dispose();
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

        private DateTime moment;

        public DateTime Moment
        {
            get
            {
                if (moment == default)
                {
                    moment = DateTime.Now;
                }
                return moment;
            }
        }

        public DbSql Sql(string str)
        {
            if (builder == null)
            {
                builder = new DbSql(str);
            }
            else
            {
                builder.Clear(); // reset
                builder.Add(str);
            }
            return builder;
        }

        public bool QueryTop(Action<IParameters> p = null, bool prepare = true)
        {
            return QueryTop(builder.ToString(), p, prepare);
        }

        public bool QueryTop(string sql, Action<IParameters> p = null, bool prepare = true)
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

        public async Task<bool> QueryTopAsync(Action<IParameters> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            multiple = false;
            command.CommandText = builder.ToString();
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }

            reader = await command.ExecuteReaderAsync();
            return reader.Read();
        }

        public async Task<bool> QueryTopAsync(string sql, Action<IParameters> p = null, bool prepare = true)
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

            reader = await command.ExecuteReaderAsync();
            return reader.Read();
        }

        public D QueryTop<D>(Action<IParameters> p = null, short msk = 0xff, bool prepare = true) where D : IData, new()
        {
            if (QueryTop(p, prepare))
            {
                return ToObject<D>(msk);
            }
            return default;
        }

        public D QueryTop<D>(string sql, Action<IParameters> p = null, short msk = 0xff, bool prepare = true) where D : IData, new()
        {
            if (QueryTop(sql, p, prepare))
            {
                return ToObject<D>(msk);
            }
            return default;
        }

        public async Task<D> QueryTopAsync<D>(Action<IParameters> p = null, short msk = 0xff, bool prepare = true) where D : IData, new()
        {
            if (await QueryTopAsync(p, prepare))
            {
                return ToObject<D>(msk);
            }
            return default;
        }

        public async Task<D> QueryTopAsync<D>(string sql, Action<IParameters> p = null, short msk = 0xff, bool prepare = true) where D : IData, new()
        {
            if (await QueryTopAsync(sql, p, prepare))
            {
                return ToObject<D>(msk);
            }
            return default;
        }

        public bool Query(Action<IParameters> p = null, bool prepare = true)
        {
            return Query(builder.ToString(), p, prepare);
        }

        public bool Query(string sql, Action<IParameters> p = null, bool prepare = true)
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

        public async Task<bool> QueryAsync(Action<IParameters> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            multiple = true;
            command.CommandText = builder.ToString();
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }

            reader = await command.ExecuteReaderAsync();
            return reader.HasRows;
        }

        public async Task<bool> QueryAsync(string sql, Action<IParameters> p = null, bool prepare = true)
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

            reader = await command.ExecuteReaderAsync();
            return reader.HasRows;
        }

        public D[] Query<D>(Action<IParameters> p = null, short msk = 0xff, bool prepare = true) where D : IData, new()
        {
            if (Query(p, prepare))
            {
                return ToArray<D>(msk);
            }

            return null;
        }

        public D[] Query<D>(string sql, Action<IParameters> p = null, short msk = 0xff, bool prepare = true) where D : IData, new()
        {
            if (Query(sql, p, prepare))
            {
                return ToArray<D>(msk);
            }
            return null;
        }

        public async Task<D[]> QueryAsync<D>(Action<IParameters> p = null, short msk = 0xff, bool prepare = true) where D : IData, new()
        {
            if (await QueryAsync(p, prepare))
            {
                return ToArray<D>(msk);
            }
            return null;
        }

        public async Task<D[]> QueryAsync<D>(string sql, Action<IParameters> p = null, short msk = 0xff, bool prepare = true) where D : IData, new()
        {
            if (await QueryAsync(sql, p, prepare))
            {
                return ToArray<D>(msk);
            }

            return null;
        }

        public Map<K, D> Query<K, D>(Action<IParameters> p = null, short msk = 0xff, Func<D, K> keyer = null, bool prepare = true) where D : IData, new()
        {
            if (Query(p, prepare))
            {
                return ToMap(msk, keyer);
            }
            return null;
        }

        public Map<K, D> Query<K, D>(string sql, Action<IParameters> p = null, short msk = 0xff, Func<D, K> keyer = null, bool prepare = true) where D : IData, new()
        {
            if (Query(sql, p, prepare))
            {
                return ToMap(msk, keyer);
            }
            return null;
        }

        public async Task<Map<K, D>> QueryAsync<K, D>(Action<IParameters> p = null, short msk = 0xff, Func<D, K> keyer = null, bool prepare = true) where D : IData, new()
        {
            if (await QueryAsync(p, prepare))
            {
                return ToMap(msk, keyer);
            }
            return null;
        }

        public async Task<Map<K, D>> QueryAsync<K, D>(string sql, Action<IParameters> p = null, short msk = 0xff, Func<D, K> keyer = null, bool prepare = true) where D : IData, new()
        {
            if (await QueryAsync(sql, p, prepare))
            {
                return ToMap(msk, keyer);
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

        public int Execute(Action<IParameters> p = null, bool prepare = true)
        {
            return Execute(builder.ToString(), p, prepare);
        }

        public int Execute(string sql, Action<IParameters> p = null, bool prepare = true)
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

        internal IParameters ReCommand(string sql = null)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            command.CommandText = sql ?? builder.ToString();
            command.CommandType = CommandType.Text;
            return this;
        }

        internal int SimpleExecute(bool prepare = true)
        {
            if (prepare) command.Prepare();
            return command.ExecuteNonQuery();
        }

        internal async Task<int> SimpleExecuteAsync(bool prepare = true)
        {
            if (prepare) command.Prepare();
            return await command.ExecuteNonQueryAsync();
        }


        public async Task<int> ExecuteAsync(Action<IParameters> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            command.CommandText = builder.ToString();
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> ExecuteAsync(string sql, Action<IParameters> p = null, bool prepare = true)
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

        public object Scalar(Action<IParameters> p = null, bool prepare = true)
        {
            return Scalar(builder.ToString(), p, prepare);
        }

        public object Scalar(string sql, Action<IParameters> p = null, bool prepare = true)
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

        public async Task<object> ScalarAsync(Action<IParameters> p = null, bool prepare = true)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Clear();
            command.CommandText = builder.ToString();
            command.CommandType = CommandType.Text;
            if (p != null)
            {
                p(this);
                if (prepare) command.Prepare();
            }
            return await command.ExecuteScalarAsync();
        }

        public async Task<object> ScalarAsync(string sql, Action<IParameters> p = null, bool prepare = true)
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

        public D ToObject<D>(short msk = 0xff) where D : IData, new()
        {
            var obj = new D();
            obj.Read(this, msk);
            return obj;
        }

        public D[] ToArray<D>(short msk = 0xff) where D : IData, new()
        {
            var lst = new ValueList<D>(32);
            while (Next())
            {
                var obj = new D();
                obj.Read(this, msk);
                lst.Add(obj);
            }
            return lst.ToArray();
        }

        public Map<K, D> ToMap<K, D>(short msk = 0xff, Func<D, K> keyer = null, Map<K, D> map = null) where D : IData, new()
        {
            while (Next())
            {
                if (map == null)
                {
                    map = new Map<K, D>(64);
                }

                var obj = new D();
                obj.Read(this, msk);
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
                    throw new DbException("Must be keyer nor IKeyable<D>");
                }
                map.Add(key, obj);
            }
            return map;
        }

        public Map<int, string> ToIntMap(Map<int, string> map = null)
        {
            while (Next())
            {
                if (map == null)
                {
                    map = new Map<int, string>(32);
                }

                Let(out int k);
                Let(out string v);

                map.Add(k, v);
            }
            return map;
        }

        public Map<short, string> ToShortMap(Map<short, string> map = null)
        {
            while (Next())
            {
                if (map == null)
                {
                    map = new Map<short, string>(32);
                }

                Let(out short k);
                Let(out string v);

                map.Add(k, v);
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
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFloat(ord);
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
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<bool[]>(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref float[] v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<float[]>(ord);
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

        public bool Get<D>(string name, ref D v, short msk = 0xff) where D : IData, new()
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<D>(ord);
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
                    var p = new JsonParser(str);
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
                    var parser = new JsonParser(str);
                    v = (JArr) parser.Parse();
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref XElem v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    var parser = new XmlParser(str);
                    v = parser.Parse();
                    return true;
                }
            }
            catch
            {
            }
            return false;
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
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<double[]>(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref decimal[] v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<decimal[]>(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref DateTime[] v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<DateTime[]>(ord);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool Get(string name, ref Guid[] v)
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<Guid[]>(ord);
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

        public bool Get<D>(string name, ref D[] v, short msk = 0xff) where D : IData, new()
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<D[]>(ord);
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

        public long[] Let(out long[] v)
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
            return v;
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

        public JObj Let(out JObj v)
        {
            v = null;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    var str = reader.GetString(ord);
                    var p = new JsonParser(str);
                    v = (JObj) p.Parse();
                }
            }
            catch
            {
            }
            return v;
        }

        public JArr Let(out JArr v)
        {
            v = null;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    var str = reader.GetString(ord);
                    var p = new JsonParser(str);
                    v = (JArr) p.Parse();
                }
            }
            catch
            {
            }
            return v;
        }

        public D Let<D>(out D v, short msk = 0xff) where D : IData, new()
        {
            v = default;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<D>(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public D[] Let<D>(out D[] v, short msk = 0xff) where D : IData, new()
        {
            v = null;
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    v = reader.GetFieldValue<D[]>(ord);
                }
            }
            catch
            {
            }

            return v;
        }

        public void Write<C>(C cnt) where C : ContentBuilder, ISink
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
            var cnt = new JsonBuilder(true, 8192);
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
            var dec = v.Decimal;
            command.Parameters.Add(new NpgsqlParameter<decimal>(name, NpgsqlDbType.Numeric)
            {
                TypedValue = dec
            });
            if (Digest)
            {
                Check(dec);
            }
        }

        public void Put(string name, bool v)
        {
            command.Parameters.Add(new NpgsqlParameter<bool>(name, NpgsqlDbType.Boolean)
            {
                TypedValue = v
            });
            if (Digest)
            {
                Check(v);
            }
        }

        public void Put(string name, char v)
        {
            command.Parameters.Add(new NpgsqlParameter<char>(name, NpgsqlDbType.Char)
            {
                TypedValue = v
            });
            if (Digest)
            {
                Check(v);
            }
        }

        public void Put(string name, short v)
        {
            command.Parameters.Add(new NpgsqlParameter<short>(name, NpgsqlDbType.Smallint)
            {
                TypedValue = v
            });
            if (Digest)
            {
                Check(v);
            }
        }

        public void Put(string name, int v)
        {
            command.Parameters.Add(new NpgsqlParameter<int>(name, NpgsqlDbType.Integer)
            {
                TypedValue = v
            });
            if (Digest)
            {
                Check(v);
            }
        }

        public void Put(string name, long v)
        {
            command.Parameters.Add(new NpgsqlParameter<long>(name, NpgsqlDbType.Bigint)
            {
                TypedValue = v
            });
            if (Digest)
            {
                Check(v);
            }
        }

        public void Put(string name, float v)
        {
            command.Parameters.Add(new NpgsqlParameter<float>(name, NpgsqlDbType.Real)
            {
                TypedValue = v
            });
            if (Digest)
            {
                Check(v);
            }
        }

        public void Put(string name, double v)
        {
            command.Parameters.Add(new NpgsqlParameter<double>(name, NpgsqlDbType.Double)
            {
                TypedValue = v
            });
            if (Digest)
            {
                Check(v);
            }
        }

        public void Put(string name, decimal v)
        {
            command.Parameters.Add(new NpgsqlParameter<decimal>(name, NpgsqlDbType.Money)
            {
                TypedValue = v
            });
            if (Digest)
            {
                Check(v);
            }
        }

        public void Put(string name, DateTime v)
        {
            if (v == default)
            {
                command.Parameters.AddWithValue(name, DBNull.Value);
            }
            else
            {
                bool notime = v.Hour == 0 && v.Minute == 0 && v.Second == 0 && v.Millisecond == 0;
                command.Parameters.Add(new NpgsqlParameter<DateTime>(name, notime ? NpgsqlDbType.Date : NpgsqlDbType.Timestamp)
                {
                    TypedValue = v
                });
                if (Digest)
                {
                    Check(v, !notime);
                }
            }
        }

        public void Put(string name, string v)
        {
            int len = v?.Length ?? 0;
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Varchar, len)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            if (Digest)
            {
                Check(v);
            }
        }

        public void Put(string name, bool[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Boolean)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            if (v != null && Digest)
            {
                foreach (var e in v)
                {
                    Check(e);
                }
            }
        }

        public void Put(string name, char[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Char)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            if (v != null && Digest)
            {
                foreach (var e in v)
                {
                    Check(e);
                }
            }
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
            if (v != null && Digest)
            {
                foreach (var e in v)
                {
                    Check(e);
                }
            }
        }

        public void Put(string name, short[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Smallint)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            if (v != null && Digest)
            {
                foreach (var e in v)
                {
                    Check(e);
                }
            }
        }

        public void Put(string name, int[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            if (v != null && Digest)
            {
                foreach (var e in v)
                {
                    Check(e);
                }
            }
            if (v != null && Digest)
            {
                foreach (var e in v)
                {
                    Check(e);
                }
            }
        }

        public void Put(string name, long[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Bigint)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            if (v != null && Digest)
            {
                foreach (var e in v)
                {
                    Check(e);
                }
            }
        }

        public void Put(string name, float[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Real)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            if (v != null && Digest)
            {
                foreach (var e in v)
                {
                    Check(e);
                }
            }
        }

        public void Put(string name, double[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Double)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            if (v != null && Digest)
            {
                foreach (var e in v)
                {
                    Check(e);
                }
            }
        }

        public void Put(string name, decimal[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Numeric)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            if (v != null && Digest)
            {
                foreach (var e in v)
                {
                    Check(e);
                }
            }
        }

        public void Put(string name, DateTime[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Date)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            if (v != null && Digest)
            {
                foreach (var e in v)
                {
                    bool notime = e.Hour == 0 && e.Minute == 0 && e.Second == 0 && e.Millisecond == 0;
                    Check(e, !notime);
                }
            }
        }

        public void Put(string name, string[] v)
        {
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            if (v != null && Digest)
            {
                foreach (var e in v)
                {
                    Check(e);
                }
            }
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
                var str = v.ToString();
                command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = str
                });
                if (Digest)
                {
                    Check(str);
                }
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
                var str = v.ToString();
                command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = str
                });
                if (Digest)
                {
                    Check(str);
                }
            }
        }

        public void Put(string name, XElem v)
        {
            if (v == null)
            {
                command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Xml)
                {
                    Value = DBNull.Value
                });
            }
            else
            {
                var str = v.ToString();
                command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Xml)
                {
                    Value = str
                });
                if (Digest)
                {
                    Check(str);
                }
            }
        }

        public void Put(string name, IData v, short msk = 0xff)
        {
            command.Parameters.Add(new NpgsqlParameter(name, (v != null) ? (object) v : DBNull.Value)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void Put<D>(string name, D[] v, short msk = 0xff) where D : IData
        {
            command.Parameters.Add(new NpgsqlParameter(name, (v != null) ? (object) v : DBNull.Value)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
        }

        public void PutFromSource(ISource s)
        {
            throw new NotImplementedException();
        }
        //
        // positional
        //

        public IParameters SetNull()
        {
            PutNull(PARAMS[paramidx++]);
            return this;
        }

        public IParameters Set(bool v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(char v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(byte v)
        {
            Put(PARAMS[paramidx++], (short) v);
            return this;
        }

        public IParameters Set(short v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters SetOrNull(short v)
        {
            if (v == 0)
            {
                PutNull(PARAMS[paramidx++]);
            }
            else
            {
                Put(PARAMS[paramidx++], v);
            }
            return this;
        }

        public IParameters Set(int v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters SetOrNull(int v)
        {
            if (v == 0)
            {
                PutNull(PARAMS[paramidx++]);
            }
            else
            {
                Put(PARAMS[paramidx++], v);
            }
            return this;
        }

        public IParameters Set(long v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters SetOrNull(long v)
        {
            if (v == 0)
            {
                PutNull(PARAMS[paramidx++]);
            }
            else
            {
                Put(PARAMS[paramidx++], v);
            }
            return this;
        }

        public IParameters Set(float v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(double v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(decimal v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(JNumber v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(DateTime v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(string v)
        {
            if (v == string.Empty)
            {
                v = null;
            }
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(bool[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(char[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(byte[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(ArraySegment<byte> v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(short[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(int[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(long[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(float[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(double[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(DateTime[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(string[] v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(JObj v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(JArr v)
        {
            Put(PARAMS[paramidx++], v);
            return this;
        }

        public IParameters Set(IData v, short msk = 0xff)
        {
            Put(PARAMS[paramidx++], v, msk);
            return this;
        }

        public IParameters Set<D>(D[] v, short msk = 0xff) where D : IData
        {
            Put(PARAMS[paramidx++], v, msk);
            return this;
        }

        public IParameters SetForIn(short[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                Put(INPARAMS[i], v[i]);
            }
            return this;
        }

        public IParameters SetForIn(int[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                Put(INPARAMS[i], v[i]);
            }
            return this;
        }

        public IParameters SetForIn<M>(IList<M> v) where M : IKeyable<int>
        {
            for (int i = 0; i < v.Count; i++)
            {
                Put(INPARAMS[i], v[i].Key);
            }
            return this;
        }

        public IParameters SetForIn(long[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                Put(INPARAMS[i], v[i]);
            }
            return this;
        }

        public IParameters SetForIn(DateTime[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                Put(INPARAMS[i], v[i]);
            }
            return this;
        }

        public IParameters SetForIn(string[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                Put(INPARAMS[i], v[i]);
            }
            return this;
        }

        public IParameters SetMoment()
        {
            Set(Moment);
            return this;
        }
        //
        // digest
        //


        public bool Digest { get; set; }

        long checksum;

        public long Checksum => checksum;

        void Check(string v)
        {
            if (v != null)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    var c = v[i];
                    CheckByte((byte) c);
                    CheckByte((byte) (c >> 8));
                }
            }
        }

        void Check(char v)
        {
            CheckByte((byte) v);
            CheckByte((byte) (v >> 8));
        }

        void Check(bool v)
        {
            CheckByte(v ? (byte) 1 : (byte) 0);
        }

        void Check(short v)
        {
            CheckByte((byte) v);
            CheckByte((byte) (v >> 8));
        }

        void Check(int v)
        {
            CheckByte((byte) v);
            CheckByte((byte) (v >> 8));
            CheckByte((byte) (v >> 16));
            CheckByte((byte) (v >> 24));
        }

        void Check(long v)
        {
            CheckByte((byte) v);
            CheckByte((byte) (v >> 8));
            CheckByte((byte) (v >> 16));
            CheckByte((byte) (v >> 24));
            CheckByte((byte) (v >> 32));
            CheckByte((byte) (v >> 40));
            CheckByte((byte) (v >> 48));
            CheckByte((byte) (v >> 56));
        }

        void Check(decimal v)
        {
            var bits = decimal.GetBits(v);
            for (int i = 0; i < bits.Length; i++)
            {
                Check(bits[i]);
            }
        }

        void Check(float v)
        {
            Check((decimal) v);
        }

        void Check(double v)
        {
            Check((decimal) v);
        }

        void Check(DateTime v, bool time)
        {
            Check(v.Year);
            Check(v.Month);
            Check(v.Day);
            if (time)
            {
                Check(v.Hour);
                Check(v.Minute);
                Check(v.Second);
                Check(v.Millisecond);
            }
        }

        void CheckByte(byte b)
        {
            var cs = checksum;
            cs ^= b << ((b & 0b00000111) * 8);
            unchecked
            {
                cs *= ((b & 0b00011000) >> 3) switch {0 => 7, 1 => 11, 2 => 13, _ => 17};
            }
            cs ^= ~b << (((b & 0b11100000) >> 5) * 8);
            checksum = cs;
        }
    }
}