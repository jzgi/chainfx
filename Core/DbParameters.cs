using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    /// <summary>
    /// A database operation session.
    /// </summary>
    public class DbParameters : IDataOutput<DbParameters>
    {
        static readonly string[] Defaults =
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24"
        };

        readonly NpgsqlParameterCollection coll;

        // current parameter index
        int position;

        internal DbParameters(NpgsqlParameterCollection coll)
        {
            this.coll = coll;
        }

        internal void Clear()
        {
            coll.Clear();
            position = 0;
        }

        public DbParameters PutNull(string name)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.AddWithValue(name, DBNull.Value);
            return this;
        }

        public DbParameters Put(string name, JNumber value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Numeric)
            {
                Value = value.Decimal
            });
            return this;
        }

        public DbParameters Put(string name, IDataInput value)
        {
            return this;
        }

        public DbParameters Put(string name, bool value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Boolean)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, short value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Smallint)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, int value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, long value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bigint)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, double value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Double)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, decimal value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, DateTime value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }

            bool date = value.Hour == 0 && value.Minute == 0 && value.Second == 0 && value.Millisecond == 0;

            coll.Add(new NpgsqlParameter(name, date ? NpgsqlDbType.Date : NpgsqlDbType.Timestamp)
            {
                Value = value
            });
            return this;
        }

        public DbParameters Put(string name, string value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            int len = value?.Length ?? 0;
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Varchar, len)
            {
                Value = (value != null) ? (object) value : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, ArraySegment<byte> value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea, value.Count)
            {
                Value = (value.Array != null) ? (object) value : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, JObj v)
        {
            if (name == null)
            {
                name = Defaults[position++];
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
                    Value = v.ToString()
                });
            }
            return this;
        }

        public DbParameters Put(string name, JArr v)
        {
            if (name == null)
            {
                name = Defaults[position++];
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
                    Value = v.ToString()
                });
            }
            return this;
        }

        public DbParameters Put(string name, short[] value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Smallint)
            {
                Value = (value != null) ? (object) value : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, int[] value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Value = (value != null) ? (object) value : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, long[] value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Bigint)
            {
                Value = (value != null) ? (object) value : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, string[] value)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = (value != null) ? (object) value : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, Dictionary<string, string> value)
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(string name, IData value, int proj = 0x00ff)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            if (value == null)
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb) {Value = DBNull.Value});
            }
            else
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = DataInputUtility.ToString(value, proj)
                });
            }
            return this;
        }

        public DbParameters Put<D>(string name, D[] value, int proj = 0x00ff) where D : IData
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            if (value == null)
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
                    Value = DataInputUtility.ToString(value, proj)
                });
            }
            return this;
        }

        //
        // positional
        //

        public DbParameters SetNull()
        {
            return PutNull(null);
        }

        public DbParameters Set(IDataInput v)
        {
            return Put(null, v);
        }

        public DbParameters Set(bool v)
        {
            return Put(null, v);
        }

        public DbParameters Set(short v)
        {
            return Put(null, v);
        }

        public DbParameters Set(int v)
        {
            return Put(null, v);
        }

        public DbParameters Set(long v)
        {
            return Put(null, v);
        }

        public DbParameters Set(double v)
        {
            return Put(null, v);
        }

        public DbParameters Set(decimal v)
        {
            return Put(null, v);
        }

        public DbParameters Set(JNumber v)
        {
            return Put(null, v);
        }

        public DbParameters Set(DateTime v)
        {
            return Put(null, v);
        }

        public DbParameters Set(string v)
        {
            return Put(null, v);
        }

        public DbParameters Set(ArraySegment<byte> v)
        {
            return Put(null, v);
        }

        public DbParameters Set(short[] v)
        {
            return Put(null, v);
        }

        public DbParameters Set(int[] v)
        {
            return Put(null, v);
        }

        public DbParameters Set(long[] v)
        {
            return Put(null, v);
        }

        public DbParameters Set(string[] v)
        {
            return Put(null, v);
        }

        public DbParameters Set(IData v, int proj = 0x00ff)
        {
            return Put(null, v);
        }

        public DbParameters Set<D>(D[] v, int proj = 0x00ff) where D : IData
        {
            return Put(null, v);
        }
    }
}