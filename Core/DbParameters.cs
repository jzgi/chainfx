using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    /// <summary>
    /// A wrapper of db parameter collection.
    /// </summary>
    public class DbParameters : ISink<DbParameters>
    {
        static string[] Params = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

        readonly NpgsqlParameterCollection coll;

        internal DbParameters(NpgsqlParameterCollection coll)
        {
            this.coll = coll;
        }

        int index; // current parameter index

        internal void Clear()
        {
            coll.Clear();
            index = 0;
        }

        public DbParameters Put(bool v)
        {
            coll.Add(new NpgsqlParameter(Params[index++], NpgsqlDbType.Boolean)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(short v)
        {
            coll.Add(new NpgsqlParameter(Params[index++], NpgsqlDbType.Smallint)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(int v)
        {
            coll.Add(new NpgsqlParameter(Params[index++], NpgsqlDbType.Integer)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(long v)
        {
            coll.Add(new NpgsqlParameter(Params[index++], NpgsqlDbType.Bigint)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(decimal v)
        {
            coll.Add(new NpgsqlParameter(Params[index++], NpgsqlDbType.Money)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(DateTime v)
        {
            NpgsqlDbType dt = (v.Hour == 0 && v.Minute == 0 && v.Second == 0 && v.Millisecond == 0) ?
                NpgsqlDbType.Date : NpgsqlDbType.Timestamp;

            coll.Add(new NpgsqlParameter(Params[index++], dt)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string v)
        {
            coll.Add(new NpgsqlParameter(Params[index++], NpgsqlDbType.Text)
            {
                Value = (v != null) ? (object)v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put<T>(T v, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public DbParameters Put<T>(List<T> v, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(byte[] v)
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(Obj v)
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(Arr v)
        {
            throw new NotImplementedException();
        }

        public DbParameters PutNull()
        {
            throw new NotImplementedException();
        }

        // SINK

        public DbParameters PutNull(string name)
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(string name, bool v)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Boolean)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, short v)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Smallint)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, int v)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, long v)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bigint)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, decimal v)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, DateTime v)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Timestamp)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, string v)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Varchar, v.Length)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put<T>(string name, T v, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public DbParameters Put<T>(string name, List<T> v, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(string name, byte[] v)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea, v.Length)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, Obj v)
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(string name, Arr v)
        {
            throw new NotImplementedException();
        }

    }
}