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

        public DbParameters Put(bool value)
        {
            coll.Add(new NpgsqlParameter(Params[index++], NpgsqlDbType.Boolean)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(short value)
        {
            coll.Add(new NpgsqlParameter(Params[index++], NpgsqlDbType.Smallint)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(int value)
        {
            coll.Add(new NpgsqlParameter(Params[index++], NpgsqlDbType.Integer)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(long value)
        {
            coll.Add(new NpgsqlParameter(Params[index++], NpgsqlDbType.Bigint)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(decimal value)
        {
            coll.Add(new NpgsqlParameter(Params[index++], NpgsqlDbType.Money)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(DateTime v)
        {
            NpgsqlDbType dt = (v.Hour == 0 && v.Minute == 0 && v.Second == 0 && v.Millisecond == 0) ?
                NpgsqlDbType.Date :
                NpgsqlDbType.Timestamp;

            coll.Add(new NpgsqlParameter(Params[index++], dt)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string value)
        {
            coll.Add(new NpgsqlParameter(Params[index++], NpgsqlDbType.Text)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put<T>(T value, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public DbParameters Put<T>(List<T> value, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(byte[] value)
        {
            throw new NotImplementedException();

        }

        public DbParameters Put(Obj value)
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(Arr value)
        {
            throw new NotImplementedException();
        }

        ////////////

        public DbParameters Put(string name, bool value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Boolean)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, short value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Smallint)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, int value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, long value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bigint)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, decimal value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, DateTime value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Timestamp)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, string value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Varchar, value.Length)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put<T>(string name, T value, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public DbParameters Put<T>(string name, List<T> value, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(string name, byte[] value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea, value.Length)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, Obj value)
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(string name, Arr value)
        {
            throw new NotImplementedException();
        }
    }
}