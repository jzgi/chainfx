using System;
using System.Data.Common;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    public static class DbParameterCollectionExtensions
    {
        public static void Set(this DbParameterCollection db, string name, bool value)
        {
            db.Add(new NpgsqlParameter(name, NpgsqlDbType.Boolean)
            {
                Value = value
            });
        }

        public static void Set(this DbParameterCollection db, string name, int value)
        {
            db.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = value
            });
        }

        public static void Set(this DbParameterCollection db, string name, decimal value)
        {
            db.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = value
            });
        }

        public static void Set(this DbParameterCollection db, string name, string value)
        {
            db.Add(new NpgsqlParameter(name, NpgsqlDbType.Text)
            {
                Value = value
            });
        }

        public static void Set(this DbParameterCollection db, string name, ArraySegment<byte> value)
        {
            db.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea)
            {
                Value = value
            });
        }

    }
}