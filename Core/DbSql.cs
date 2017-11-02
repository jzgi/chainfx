using System;

namespace Greatbone.Core
{
    /// <summary>
    /// A specialized string builder for generating SQL commands.
    /// </summary>
    public class DbSql : DynamicContent, IDataOutput<DbSql>
    {
        // contexts
        const sbyte CTX_COLUMNLIST = 1, CTX_PARAMLIST = 2, CTX_SETLIST = 3;

        // the putting context
        internal sbyte ctx;

        // used when generating a list
        internal int ordinal;

        internal DbSql(string str) : base(false, 1024)
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

        public DbSql T(string str)
        {
            Add(str);
            return this;
        }

        public DbSql setlst(IData obj, short proj = 0x00ff)
        {
            ctx = CTX_SETLIST;
            ordinal = 1;
            obj.Write(this, proj);
            return this;
        }

        public DbSql columnlst(IData obj, short proj = 0x00ff)
        {
            ctx = CTX_COLUMNLIST;
            ordinal = 1;
            obj.Write(this, proj);
            return this;
        }

        public DbSql paramlst(IData obj, short proj = 0x00ff)
        {
            ctx = CTX_PARAMLIST;
            ordinal = 1;
            obj.Write(this, proj);
            return this;
        }

        public DbSql _(IData obj, short proj = 0x00ff, string extra = null)
        {
            Add(" (");
            columnlst(obj, proj);
            if (extra != null)
            {
                Add(",");
                Add(extra);
            }
            Add(")");
            return this;
        }

        public DbSql _VALUES_(short n)
        {
            Add(" VALUES (");
            for (short i = 1; i <= n; i++)
            {
                if (i > 1)
                {
                    Add(',');
                    Add(' ');
                }
                Add('@');
                Add(i);
            }
            Add(")");
            return this;
        }

        public DbSql _VALUES_(IData obj, short proj = 0x00ff, string extra = null)
        {
            Add(" VALUES (");
            paramlst(obj, proj);
            if (extra != null)
            {
                Add(",");
                Add(extra);
            }
            Add(")");
            return this;
        }

        public DbSql _SET_(IData obj, short proj = 0x00ff, string extra = null)
        {
            Add(" SET ");
            setlst(obj, proj);
            if (extra != null)
            {
                Add(",");
                Add(extra);
            }
            return this;
        }

        public DbSql _IN_(int[] vals)
        {
            Add(" IN (");
            for (int i = 0; i < vals.Length; i++)
            {
                if (i > 0) Add(',');
                Add(vals[i]);
            }
            Add(')');

            return this;
        }

        public DbSql _IN_(long[] vals)
        {
            Add(" IN (");
            for (int i = 0; i < vals.Length; i++)
            {
                if (i > 0) Add(',');
                Add(vals[i]);
            }
            Add(')');

            return this;
        }

        public DbSql _IN_(string[] vals)
        {
            Add(" IN (");
            for (int i = 0; i < vals.Length; i++)
            {
                if (i > 0) Add(',');
                Add('\'');
                Add(vals[i]);
                Add('\'');
            }
            Add(')');

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

        public DbSql Put(string name, string v)
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

        public DbSql Put(string name, ArraySegment<byte> v)
        {
            Build(name);
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

        public DbSql Put(string name, Map<string, string> v)
        {
            throw new NotImplementedException();
        }

        public DbSql Put(string name, IData v, short proj = 0x00ff)
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

        public DbSql Put<D>(string name, D[] v, short proj = 0x00ff) where D : IData
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