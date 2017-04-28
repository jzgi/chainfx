using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// A specialized string builder for generating SQL commands.
    ///
    public class DbSql : DynamicContent, IDataOutput<DbSql>
    {
        // contexts
        const sbyte CTX_COLUMNLIST = 1, CTX_PARAMLIST = 2, CTX_SETLIST = 3;

        // the putting context
        internal sbyte ctx;

        // used when generating a list
        internal int ordinal;

        internal DbSql(string str) : base(false, true, 1024)
        {
            Add(str);
        }

        public override string Type => "text/plain";

        internal void Clear()
        {
            count = 0;
            ctx = 0;
            ordinal = 0;
        }

        public DbSql _(string str)
        {
            Add(' ');
            Add(str);

            return this;
        }

        public DbSql setlst(IData obj, short proj = 0)
        {
            ctx = CTX_SETLIST;
            ordinal = 1;
            obj.WriteData(this, proj);
            return this;
        }

        public DbSql setstate()
        {
            return this;
        }

        public DbSql statecond()
        {
            // 
            return this;
        }

        public DbSql columnlst(IData obj, short proj = 0)
        {
            ctx = CTX_COLUMNLIST;
            ordinal = 1;
            obj.WriteData(this, proj);
            return this;
        }

        public DbSql parameterlst(IData obj, short proj = 0)
        {
            ctx = CTX_PARAMLIST;
            ordinal = 1;
            obj.WriteData(this, proj);
            return this;
        }

        public DbSql _(IData obj, short proj = 0)
        {
            Add(" (");
            columnlst(obj, proj);
            Add(")");
            return this;
        }

        public DbSql _VALUES_(IData obj, short proj = 0)
        {
            Add(" VALUES (");
            parameterlst(obj, proj);
            Add(")");
            return this;
        }

        public DbSql _SET_(IData obj, short proj = 0)
        {
            Add(" SET ");
            setlst(obj, proj);
            return this;
        }

        void Build(string name)
        {
            if (ordinal > 1) Add(", ");

            switch (ctx)
            {
                case CTX_COLUMNLIST:
                    Add('"');
                    Add(name);
                    Add('"');
                    break;
                case CTX_PARAMLIST:
                    Add("@");
                    Add(name);
                    break;
                case CTX_SETLIST:
                    Add('"');
                    Add(name);
                    Add('"');
                    Add("=@");
                    Add(name);
                    break;
            }

            ordinal++;
        }

        public DbSql PutNull(string name)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, JNumber v)
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

        public DbSql Put(string name, IDataInput v)
        {
            return this;
        }

        public DbSql PutRaw(string name, string raw)
        {
            return this;
        }

        public void Group(string label)
        {
        }

        public void UnGroup()
        {
        }

        public DbSql Put(string name, bool v, Func<bool, string> Options = null, string Label = null, bool Required = false)
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

        public DbSql Put(string name, short v, Opt<short> opt = null, string Label = null, string Help = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbSql Put(string name, int v, Opt<int> Opt = null, string Label = null, string Help = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbSql Put(string name, long v, Opt<long> Opt = null, string Label = null, string Help = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbSql Put(string name, double v, string label = null, string Help = null, double Max = 0, double Min = 0, double Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbSql Put(string name, decimal v, string label = null, string Help = null, decimal Max = 0, decimal Min = 0, decimal Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbSql Put(string name, DateTime v, string label = null, DateTime Max = default(DateTime), DateTime Min = default(DateTime), int Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbSql Put(string name, string v, Opt<string> Opt = null, string Label = null, string Help = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
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

        public DbSql Put(string name, ArraySegment<byte> v, string label = null, string size = null, string ratio = null, bool Required = false)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, short[] v, Opt<short> opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
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

        public DbSql Put(string name, int[] v, Opt<int> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
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

        public DbSql Put(string name, long[] v, Opt<long> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
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

        public DbSql Put(string name, string[] v, Opt<string> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
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

        public DbSql Put(string name, Dictionary<string, string> v, string label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            throw new NotImplementedException();
        }

        public DbSql Put(string name, IData v, short proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
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

        public DbSql Put<D>(string name, D[] v, short proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool required = false) where D : IData
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