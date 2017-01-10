using System;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A wrapper of db parameter collection.
    ///
    public class DbParameters : ISink<DbParameters>
    {
        static readonly string[] Defaults =
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "15", "16", "17", "18", "19", "20"
        };

        readonly NpgsqlParameterCollection coll;

        // current parameter index
        int index;

        internal DbParameters(NpgsqlParameterCollection coll)
        {
            this.coll = coll;
        }

        internal void Clear()
        {
            coll.Clear();
            index = 0;
        }


        public DbParameters PutNull(string name)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.AddWithValue(name, DBNull.Value);
            return this;
        }

        public DbParameters Put(string name, bool v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Boolean)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, short v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Smallint)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, int v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, long v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bigint)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, double v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Double)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, decimal v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, JNumber v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Numeric)
            {
                Value = v.Decimal
            });
            return this;
        }

        public DbParameters Put(string name, DateTime v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Timestamp)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, NpgsqlPoint v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Point)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, char[] v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Char, v.Length)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, string v, bool? anylen = null)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, !anylen.HasValue ? NpgsqlDbType.Varchar : anylen.Value ? NpgsqlDbType.Text : NpgsqlDbType.Char)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, byte[] v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, ArraySegment<byte> v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea, v.Count)
            {
                Value = (v.Array != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put<D>(string name, D v, byte flags = 0) where D : IData
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            if (v == null)
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb) {Value = DBNull.Value});
            }
            else
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = JsonUtility.ObjectToString(v, flags)
                });
            }
            return this;
        }

        public DbParameters Put(string name, JObj v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            if (v == null)
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = DBNull.Value
                });
            }
            else
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = JsonUtility.JObjToString(v)
                });
            }
            return this;
        }

        public DbParameters Put(string name, JArr v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            if (v == null)
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = DBNull.Value
                });
            }
            else
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = JsonUtility.JArrToString(v)
                });
            }
            return this;
        }

        public DbParameters Put(string name, short[] v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Smallint)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, int[] v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, long[] v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Bigint)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, string[] v)
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put<D>(string name, D[] v, byte flags = 0) where D : IData
        {
            if (name == null)
            {
                name = Defaults[index++];
            }
            if (v == null)
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = DBNull.Value
                });
            }
            else
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = JsonUtility.ArrayToString(v, flags)
                });
            }
            return this;
        }
    }
}