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

        public DbParameters Put(string name, bool v, Func<bool, string> Opt = null, string Label = null, bool Required = false)
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

        public DbParameters Put(string name, short v, Set<short> Opt = null, string Label = null, string Help = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbParameters Put(string name, int v, Set<int> Opt = null, string Label = null, string Help = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbParameters Put(string name, long v, Set<long> Opt = null, string Label = null, string Help = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbParameters Put(string name, double v, string Label = null, string Help = null, double Max = 0, double Min = 0, double Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbParameters Put(string name, decimal v, string Label = null, string Help = null, decimal Max = 0, decimal Min = 0, decimal Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbParameters Put(string name, DateTime v, string Label = null, DateTime Max = default(DateTime), DateTime Min = default(DateTime), int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Timestamp)
            {
                Value = v
            });
            return this;
        }

        public DbParameters Put(string name, string v, Set<string> Opt = null, string Label = null, string Help = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            int len = v?.Length ?? 0;
            coll.Add(new NpgsqlParameter(name, Max < 126 ? NpgsqlDbType.Varchar : NpgsqlDbType.Text, len)
            {
                Value = (v != null) ? (object)v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, ArraySegment<byte> v, string Label = null, string Size = null, string Ratio = null, bool Required = false)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Bytea, v.Count)
            {
                Value = (v.Array != null) ? (object)v : DBNull.Value
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
                    Value = JsonUtility.JObjToString(v)
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
                    Value = JsonUtility.JArrToString(v)
                });
            }
            return this;
        }

        public DbParameters Put(string name, short[] v, Set<short> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Smallint)
            {
                Value = (v != null) ? (object)v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, int[] v, Set<int> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Value = (v != null) ? (object)v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, long[] v, Set<long> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Bigint)
            {
                Value = (v != null) ? (object)v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, string[] v, Set<string> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Value = (v != null) ? (object)v : DBNull.Value
            });
            return this;
        }

        public DbParameters Put(string name, Map v, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            throw new NotImplementedException();
        }

        public DbParameters Put(string name, IData v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            if (name == null)
            {
                name = Defaults[position++];
            }
            if (v == null)
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb) { Value = DBNull.Value });
            }
            else
            {
                coll.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
                {
                    Value = JsonUtility.ObjectToString(v, proj)
                });
            }
            return this;
        }

        public DbParameters Put<D>(string name, D[] v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false) where D : IData
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
                    Value = JsonUtility.ArrayToString(v, proj)
                });
            }
            return this;
        }

        public DbParameters Put<D>(string name, List<D> v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false) where D : IData
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
                    Value = JsonUtility.ListToString(v, proj)
                });
            }
            return this;
        }

        public DbParameters Put<D>(string name, Map<D> v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false) where D : IData
        {
            throw new NotImplementedException();
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

        public DbParameters Set(IData v, int proj = 0)
        {
            return Put(null, v);
        }

        public DbParameters Set<D>(D[] v, int proj = 0) where D : IData
        {
            return Put(null, v);
        }

        public DbParameters Set<D>(List<D> v, int proj = 0) where D : IData
        {
            return Put(null, v);
        }
    }
}