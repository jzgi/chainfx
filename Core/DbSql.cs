using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A helper used to generate SQL commands.
    ///
    public class DbSql : DynamicContent, ISink<DbSql>
    {
        // contexts
        const sbyte ColumnList = 1, ParameterList = 2, SetList = 3;

        // the putting context
        internal sbyte list;

        // used when generating a list
        internal int ordinal;

        public DbSql(bool sendable, bool poolable, int capacity = 1024) : base(sendable, poolable, capacity)
        {
        }

        public DbSql(string str) : base(false, true, 1024)
        {
            Add(str);
        }

        public override string MimeType => null;

        internal void Clear()
        {
            count = 0;
            list = 0;
            ordinal = 0;
        }

        public DbSql _(string str)
        {
            Add(' ');
            Add(str);

            return this;
        }

        public DbSql setlst<T>(T obj, byte bits = 0) where T : IData
        {
            list = SetList;
            ordinal = 1;
            obj.Dump(this, bits);
            return this;
        }

        public DbSql columnlst<T>(T obj, byte bits = 0) where T : IData
        {
            list = ColumnList;
            ordinal = 1;
            obj.Dump(this, bits);
            return this;
        }

        public DbSql parameterlst<T>(T obj, byte bits = 0) where T : IData
        {
            list = ParameterList;
            ordinal = 1;
            obj.Dump(this, bits);
            return this;
        }

        public DbSql _<T>(T obj, byte bits = 0) where T : IData
        {
            Add(" (");
            columnlst(obj, bits);
            Add(")");
            return this;
        }

        public DbSql _VALUES_<T>(T obj, byte z = 0) where T : IData
        {
            Add(" VALUES (");
            parameterlst(obj, z);
            Add(")");
            return this;
        }

        public DbSql _SET_<T>(T obj, byte bits = 0) where T : IData
        {
            Add(" SET ");
            setlst(obj, bits);
            return this;
        }

        void Build(string name)
        {
            if (ordinal > 1) Add(", ");

            switch (list)
            {
                case ColumnList:
                    Add('"');
                    Add(name);
                    Add('"');
                    break;
                case ParameterList:
                    Add("@");
                    Add(name);
                    break;
                case SetList:
                    Add('"');
                    Add(name);
                    Add('"');
                    Add("=@");
                    Add(name);
                    break;
            }

            ordinal++;
        }

        public DbSql Put(string name, bool v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v ? "TRUE" : "FALSE");
            }
            return this;
        }

        public DbSql Put(string name, short v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
            return this;
        }

        public DbSql Put(string name, int v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
            return this;
        }

        public DbSql Put(string name, long v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
            return this;
        }

        public DbSql Put(string name, double v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
            return this;
        }

        public DbSql Put(string name, Number v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
            return this;
        }

        public DbSql Put(string name, DateTime v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
            return this;
        }

        public DbSql Put(string name, NpgsqlPoint v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
            return this;
        }

        public DbSql Put(string name, decimal v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
            return this;
        }

        public DbSql Put(string name, string v, bool? anylen = null)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add('\'');
                Add(v);
                Add('\'');
            }
            return this;
        }

        public DbSql Put(string name, char[] v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add('\'');
                Add(v);
                Add('\'');
            }
            return this;
        }

        public DbSql Put(string name, byte[] v)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, ArraySegment<byte> v)
        {
            Build(name);
            return this;
        }

        public DbSql Put<D>(string name, D v, byte bits = 0) where D : IData
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (v == null)
                {
                    Add("NULL");
                }
                else
                {
                }
            }
            return this;
        }

        public DbSql Put(string name, Arr v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (v == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add('\'');
                    v.Dump(this);
                    Add('\'');
                }
            }
            return this;
        }

        public DbSql Put(string name, Obj v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (v == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add('\'');
                    v.Dump(this);
                    Add('\'');
                }
            }
            return this;
        }

        public DbSql Put(string name, short[] v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (v == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add("ARRAY[");
                    for (int i = 0; i < v.Length; i++)
                    {
                        if (i > 0) Add(',');
                        Add(v[i]);
                    }
                    Add(']');
                    if (v.Length == 0)
                    {
                        Add("::smallint[]");
                    }
                }
            }
            return this;
        }

        public DbSql Put(string name, int[] v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (v == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add("ARRAY[");
                    for (int i = 0; i < v.Length; i++)
                    {
                        if (i > 0) Add(',');
                        Add(v[i]);
                    }
                    Add(']');
                    if (v.Length == 0)
                    {
                        Add("::integer[]");
                    }
                }
            }
            return this;
        }

        public DbSql Put(string name, string[] v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (v == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add("ARRAY[");
                    for (int i = 0; i < v.Length; i++)
                    {
                        if (i > 0) Add(',');
                        Add('\'');
                        Add(v[i]);
                        Add('\'');
                    }
                    Add(']');
                    if (v.Length == 0)
                    {
                        Add("::varchar[]");
                    }
                }
            }
            return this;
        }

        public DbSql Put(string name, long[] v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (v == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add("ARRAY[");
                    for (int i = 0; i < v.Length; i++)
                    {
                        if (i > 0) Add(',');
                        Add(v[i]);
                    }
                    Add(']');
                    if (v.Length == 0)
                    {
                        Add("::bigint[]");
                    }
                }
            }
            return this;
        }

        public DbSql Put<D>(string name, D[] v, byte bits = 0) where D : IData
        {
            Build(name);
            return this;
        }

        public DbSql PutNull(string name)
        {
            Build(name);
            return this;
        }

        public override string ToString()
        {
            return new string(charbuf, 0, count);
        }
    }
}