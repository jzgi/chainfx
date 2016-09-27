using System;
using System.Collections.Generic;
using System.Data.Common;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    public static class DbParameterCollectionExtensions
    {
        public static DbParameterCollection Set(this DbParameterCollection coll, string name, bool value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Boolean)
            {
                Value = value
            });
            return coll;
        }

        public static DbParameterCollection Set(this DbParameterCollection coll, string name, int value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = value
            });
            return coll;
        }

        public static DbParameterCollection Set(this DbParameterCollection coll, string name, decimal value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = value
            });
            return coll;
        }

        public static DbParameterCollection Set(this DbParameterCollection coll, string name, string value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Text)
            {
                Value = value
            });
            return coll;
        }

        public static DbParameterCollection Set(this DbParameterCollection coll, string name, ArraySegment<byte> value)
        {
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea)
            {
                Value = value
            });
            return coll;
        }

        public static DbParameterCollection Set<T>(this DbParameterCollection coll, string name, List<T> value) where T : IDat
        {
            JsonText text = new JsonText(value.ToString());

            // text.Write

            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
            {
                Value = text.ToString()
            });
            return coll;
        }

    }
}