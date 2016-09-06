using System;
using System.Data.Common;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    public static class DbParameterCollectionExtensions
    {
        public static void Set(this DbParameterCollection coll, string name, bool value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Boolean)
            {
                Value = value
            });
        }

        public static void Set(this DbParameterCollection coll, string name, int value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = value
            });
        }

        public static void Set(this DbParameterCollection coll, string name, decimal value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = value
            });
        }

        public static void Set(this DbParameterCollection coll, string name, string value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Text)
            {
                Value = value
            });
        }

        public static void Set(this DbParameterCollection coll, string name, ArraySegment<byte> value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea)
            {
                Value = value
            });
        }

    }
}