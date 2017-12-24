using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    /// <summary>
    /// An environment for database operations based on current service.
    /// </summary>
    public class DbContext : IDataInput, IDbParams, IDisposable
    {
        static readonly string[] PARAMS =
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24"
        };

        readonly Service service;

        readonly IDoerContext<IDoer> doerCtx;

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

        internal DbContext(Service service, IDoerContext<IDoer> doerCtx = null)
        {
            this.service = service;
            this.doerCtx = doerCtx;
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
                reader.Dispose();
                reader = null;
            }
            ordinal = 0;
            command.Parameters.Clear();
            index = 0;
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

        public bool Query1(Action<IDbParams> p = null, bool prepare = true)
        {
            return Query1(sql.ToString(), p, prepare);
        }

        public bool Query1(DbSql sql, Action<IDbParams> p = null, bool prepare = true)
        {
            this.sql = sql;
            return Query1(sql.ToString(), p, prepare);
        }

        public D Query1<D>(DbSql sql, Action<IDbParams> p = null, short proj = 0x00ff, bool prepare = true) where D : IData, new()
        {
            this.sql = sql;
            if (Query1(sql.ToString(), p, prepare))
            {
                return ToObject<D>(proj);
            }
            return default;
        }

        public D Query1<D>(string cmdtext, Action<IDbParams> p = null, short proj = 0x00ff, bool prepare = true) where D : IData, new()
        {
            if (Query1(cmdtext, p, prepare))
            {
                return ToObject<D>(proj);
            }
            return default;
        }

        public bool Query1(string cmdtext, Action<IDbParams> p = null, bool prepare = true)
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
                p(this);
                if (prepare) command.Prepare();
            }
            reader = command.ExecuteReader();
            return reader.Read();
        }

        public bool Query(Action<IDbParams> p = null, bool prepare = true)
        {
            return Query(sql.ToString(), p, prepare);
        }

        public bool Query(DbSql sql, Action<IDbParams> p = null, bool prepare = true)
        {
            this.sql = sql;
            return Query(sql.ToString(), p, prepare);
        }

        public D[] Query<D>(DbSql sql, Action<IDbParams> p = null, short proj = 0x00ff, bool prepare = true) where D : IData, new()
        {
            this.sql = sql;
            if (Query(sql.ToString(), p, prepare))
            {
                return ToArray<D>(proj);
            }
            return null;
        }

        public D[] Query<D>(string cmdtext, Action<IDbParams> p = null, short proj = 0x00ff, bool prepare = true) where D : IData, new()
        {
            if (Query(cmdtext, p, prepare))
            {
                return ToArray<D>(proj);
            }
            return null;
        }

        public bool Query(string cmdtext, Action<IDbParams> p = null, bool prepare = true)
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
                p(this);
                if (prepare) command.Prepare();
            }
            reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public async Task<bool> QueryAsync(DbSql sql, Action<IDbParams> p = null, bool prepare = true)
        {
            this.sql = sql;
            return await QueryAsync(sql.ToString(), p, prepare);
        }

        public async Task<bool> QueryAsync(string cmdtext, Action<IDbParams> p = null, bool prepare = true)
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
                p(this);
                if (prepare) command.Prepare();
            }
            reader = (NpgsqlDataReader) await command.ExecuteReaderAsync();
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

        public int Execute(Action<IDbParams> p = null, bool prepare = true)
        {
            return Execute(sql.ToString(), p, prepare);
        }

        public int Execute(DbSql sql, Action<IDbParams> p = null, bool prepare = true)
        {
            this.sql = sql;
            return Execute(sql.ToString(), p, prepare);
        }

        public int Execute(string cmdtext, Action<IDbParams> p = null, bool prepare = true)
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
                p(this);
                if (prepare) command.Prepare();
            }
            return command.ExecuteNonQuery();
        }

        public async Task<int> ExecuteAsync(Action<IDbParams> p = null, bool prepare = true)
        {
            return await ExecuteAsync(sql.ToString(), p, prepare);
        }

        public async Task<int> ExecuteAsync(string cmdtext, Action<IDbParams> p = null, bool prepare = true)
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
                p(this);
                if (prepare) command.Prepare();
            }
            return await command.ExecuteNonQueryAsync();
        }

        public object Scalar(Action<IDbParams> p = null, bool prepare = true)
        {
            return Scalar(sql.ToString(), p, prepare);
        }

        public object Scalar(DbSql sql, Action<IDbParams> p = null, bool prepare = true)
        {
            this.sql = sql;
            return Scalar(sql.ToString(), p, prepare);
        }

        public object Scalar(string cmdtext, Action<IDbParams> p = null, bool prepare = true)
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
                p(this);
                if (prepare) command.Prepare();
            }
            object res = command.ExecuteScalar();
            return res == DBNull.Value ? null : res;
        }

        public async Task<object> ScalarAsync(Action<IDbParams> p = null, bool prepare = true)
        {
            return await ScalarAsync(sql.ToString(), p, prepare);
        }

        public async Task<object> ScalarAsync(string cmdtext, Action<IDbParams> p = null, bool prepare = true)
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
                p(this);
                if (prepare) command.Prepare();
            }
            return await command.ExecuteScalarAsync();
        }

        // TODO
        public async Task<object> CallAsync(string storedproc, Action<IDbParams> p = null, bool prepare = true)
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
                p(this);
                if (prepare) command.Prepare();
            }
            return await command.ExecuteScalarAsync();
        }

        //
        // RESULTSET
        //

        public D ToObject<D>(short proj = 0x00ff) where D : IData, new()
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

        public D[] ToArray<D>(short proj = 0x00ff) where D : IData, new()
        {
            List<D> coll = null;
            while (Next())
            {
                D obj = new D();
                obj.Read(this, proj);

                // add shard if any
                if (obj is IShardable sharded)
                {
                    sharded.Shard = service.Shard;
                }

                if (coll == null)
                {
                    coll = new List<D>(32);
                }
                coll.Add(obj);
            }
            return coll?.ToArray();
        }

        public List<D> ToList<D>(short proj = 0x00ff) where D : IData, new()
        {
            List<D> coll = new List<D>(32);
            while (Next())
            {
                D obj = new D();
                obj.Read(this, proj);

                // add shard if any
                if (obj is IShardable sharded)
                {
                    sharded.Shard = service.Shard;
                }

                coll.Add(obj);
            }
            return coll;
        }

        public Map<K, D> ToMap<K, D>(Func<D, K> keyer, short proj = 0x00ff) where D : IData, new()
        {
            Map<K, D> coll = new Map<K, D>();
            while (Next())
            {
                D obj = new D();
                obj.Read(this, proj);

                // add shard name if any
                if (obj is IShardable sharded)
                {
                    sharded.Shard = service.Shard;
                }

                K key = keyer(obj);
                coll.Add(key, obj);
            }
            return coll;
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

        public bool Get<D>(string name, ref D v, short proj = 0x00ff) where D : IData, new()
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParse p = new JsonParse(str);
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
                    JsonParse p = new JsonParse(str);
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
                    JsonParse parse = new JsonParse(str);
                    v = (JArr) parse.Parse();
                    return true;
                }
            }
            catch
            {
            }
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

        public bool Get<D>(string name, ref D[] v, short proj = 0x00ff) where D : IData, new()
        {
            try
            {
                int ord = reader.GetOrdinal(name);
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParse parse = new JsonParse(str);
                    JArr ja = (JArr) parse.Parse();
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

        public IDataInput Let(out bool v)
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

        public IDataInput Let(out short v)
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

        public IDataInput Let(out int v)
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

        public IDataInput Let(out long v)
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

        public IDataInput Let(out double v)
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

        public IDataInput Let(out decimal v)
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

        public IDataInput Let(out DateTime v)
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

        public IDataInput Let(out string v)
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

        public IDataInput Let(out ArraySegment<byte> v)
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

        public IDataInput Let(out short[] v)
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

        public IDataInput Let(out int[] v)
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

        public IDataInput Let(out long[] v)
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

        public IDataInput Let(out string[] v)
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

        public IDataInput Let(out Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public IDataInput Let<D>(out D v, short proj = 0x00ff) where D : IData, new()
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParse p = new JsonParse(str);
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

        public IDataInput Let<D>(out D[] v, short proj = 0x00ff) where D : IData, new()
        {
            try
            {
                int ord = ordinal++;
                if (!reader.IsDBNull(ord))
                {
                    string str = reader.GetString(ord);
                    JsonParse parse = new JsonParse(str);
                    JArr ja = (JArr) parse.Parse();
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


        //
        // PARAMETERS

        public IDbParams PutNull(string name)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.AddWithValue(name, DBNull.Value);
            return this;
        }

        public IDbParams Put(string name, JNumber v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Numeric)
            {
                Value = v.Decimal
            });
            return this;
        }

        public IDbParams Put(string name, IDataInput v)
        {
            return this;
        }

        public IDbParams Put(string name, bool v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Boolean)
            {
                Value = v
            });
            return this;
        }

        public IDbParams Put(string name, short v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Smallint)
            {
                Value = v
            });
            return this;
        }

        public IDbParams Put(string name, int v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = v
            });
            return this;
        }

        public IDbParams Put(string name, long v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Bigint)
            {
                Value = v
            });
            return this;
        }

        public IDbParams Put(string name, double v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Double)
            {
                Value = v
            });
            return this;
        }

        public IDbParams Put(string name, decimal v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = v
            });
            return this;
        }

        public IDbParams Put(string name, DateTime v)
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
            return this;
        }

        public IDbParams Put(string name, string v)
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
            return this;
        }

        public IDbParams Put(string name, ArraySegment<byte> v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea, v.Count)
            {
                Value = (v.Array != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public IDbParams Put(string name, JObj v)
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
            return this;
        }

        public IDbParams Put(string name, JArr v)
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
            return this;
        }

        public IDbParams Put(string name, short[] v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Smallint)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public IDbParams Put(string name, int[] v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public IDbParams Put(string name, long[] v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Bigint)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public IDbParams Put(string name, string[] v)
        {
            if (name == null)
            {
                name = PARAMS[index++];
            }
            command.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public IDbParams Put(string name, Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public IDbParams Put(string name, IData v, short proj = 0x00ff)
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
                    Value = DataInputUtility.ToString(v, proj)
                });
            }
            return this;
        }

        public IDbParams Put<D>(string name, D[] v, short proj = 0x00ff) where D : IData
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
                    Value = DataInputUtility.ToString(v, proj)
                });
            }
            return this;
        }

        //
        // positional
        //

        public IDbParams SetNull()
        {
            return PutNull(null);
        }

        public IDbParams Set(IDataInput v)
        {
            return Put(null, v);
        }

        public IDbParams Set(bool v)
        {
            return Put(null, v);
        }

        public IDbParams Set(short v)
        {
            return Put(null, v);
        }

        public IDbParams Set(int v)
        {
            return Put(null, v);
        }

        public IDbParams Set(long v)
        {
            return Put(null, v);
        }

        public IDbParams Set(double v)
        {
            return Put(null, v);
        }

        public IDbParams Set(decimal v)
        {
            return Put(null, v);
        }

        public IDbParams Set(JNumber v)
        {
            return Put(null, v);
        }

        public IDbParams Set(DateTime v)
        {
            return Put(null, v);
        }

        public IDbParams Set(string v)
        {
            return Put(null, v);
        }

        public IDbParams Set(ArraySegment<byte> v)
        {
            return Put(null, v);
        }

        public IDbParams Set(short[] v)
        {
            return Put(null, v);
        }

        public IDbParams Set(int[] v)
        {
            return Put(null, v);
        }

        public IDbParams Set(long[] v)
        {
            return Put(null, v);
        }

        public IDbParams Set(string[] v)
        {
            return Put(null, v);
        }

        public IDbParams Set(IData v, short proj = 0x00ff)
        {
            return Put(null, v, proj);
        }

        public IDbParams Set<D>(D[] v, short proj = 0x00ff) where D : IData
        {
            return Put(null, v, proj);
        }

        //
        // EVENTS
        //

        public void Publish(string name, string shard, int arg, IDataInput inp)
        {
            DynamicContent dcont = inp.Dump();
            Publish(name, shard, arg, dcont);
            BufferUtility.Return(dcont); // back to pool
        }

        public void Publish(string name, string shard, int arg, IData obj, short proj = 0x00ff)
        {
            JsonContent cont = new JsonContent(true).Put(null, obj, proj);
            Publish(name, shard, arg, cont);
            BufferUtility.Return(cont); // back to pool
        }

        public void Publish<D>(string name, string shard, int arg, D[] arr, short proj = 0x00ff) where D : IData
        {
            JsonContent cont = new JsonContent(true).Put(null, arr, proj);
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

        public void Write<R>(IDataOutput<R> o) where R : IDataOutput<R>
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

        public DynamicContent Dump()
        {
            return new JsonContent(true).Put(null, this);
        }

        public void Rollback()
        {
            if (transact != null && !transact.IsCompleted)
            {
                // indicate disposing the instance 
                reader?.Close();
                command.Dispose();
                transact.Rollback();
                command.Transaction = null;
                transact = null;
            }
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
                    transact.Commit();
                }
                reader?.Close();
                command.Dispose();
                connection.Close();
            }
        }
    }
}