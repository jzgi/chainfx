using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A database operation session.
    ///
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

        public DbParameters Put(string name, JNumber v)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Numeric)
            {
                Value = v.Decimal
            });
            return this;
        }

        public DbParameters Put(string name, IDataInput v)
        {
            return this;
        }

        public DbParameters PutRaw(string name, string raw)
        {
            return this;
        }

        public void Group(string label)
        {
        }

        public void UnGroup()
        {
        }

        public DbParameters Put(string name, bool v, string label = null, Func<bool, string> Opt = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Boolean)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, short v, string label = null, IOptable<short> opt = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Smallint)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, int v, string label = null, IOptable<int> Opt = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Integer)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, long v, string label = null, IOptable<long> opt = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bigint)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, double v, string Label = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Double)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, decimal v, string Label = null, char format = '\0')
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, DateTime v, string Label = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }

            bool date = v.Hour == 0 && v.Minute == 0 && v.Second == 0 && v.Millisecond == 0;

            coll.Add(new NpgsqlParameter(name, date ? NpgsqlDbType.Date : NpgsqlDbType.Timestamp)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, string v, string label = null, IOptable<string> Opt = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            int len = v?.Length ?? 0;
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Varchar, len)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, ArraySegment<byte> v, string Label = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea, v.Count)
            {
                Value = (v.Array != null) ? (object) v : DBNull.Value
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

        public DbParameters Put(string name, short[] v, string label = null, IOptable<short> opt = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Smallint)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, int[] v, string label = null, IOptable<int> Opt = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, long[] v, string label = null, IOptable<long> Opt = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Bigint)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, string[] v, string label = null, IOptable<string> Opt = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = (v != null) ? (object) v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, Dictionary<string, string> v, string Label = null)
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(string name, IData v, int proj = 0x00ff, string label = null)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            if (v == null)
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb) {Value = DBNull.Value});
            }
            else
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = DataInputUtility.ToString(v, proj)
                });
            }
            return this;
        }

        public DbParameters Put<D>(string name, D[] v, int proj = 0x00ff, string label = null) where D : IData
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
                    Value = DataInputUtility.ToString(v, proj)
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