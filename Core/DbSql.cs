using System;
using System.Collections.Generic;

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

        public DbSql(string str) : base(false, 1024)
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

        public DbSql comma_(string str)
        {
            Add(", ");
            Add(str);
            return this;
        }

        public DbSql @params(sbyte n)
        {
            for (int i = 0; i < n; i++)
            {
                if (i > 0)
                {
                    Add(',');
                    Add(' ');
                }
                Add('@');
                Add(i);
            }
            return this;
        }

        public DbSql setlst(IData obj, int proj = 0x00ff)
        {
            ctx = CTX_SETLIST;
            ordinal = 1;
            obj.Write(this, proj);
            return this;
        }

        public DbSql columnlst(IData obj, int proj = 0x00ff)
        {
            ctx = CTX_COLUMNLIST;
            ordinal = 1;
            obj.Write(this, proj);
            return this;
        }

        public DbSql parameterlst(IData obj, int proj = 0x00ff)
        {
            ctx = CTX_PARAMLIST;
            ordinal = 1;
            obj.Write(this, proj);
            return this;
        }

        public DbSql _(IData obj, int proj = 0x00ff, string extra = null)
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

        public DbSql _VALUES_(IData obj, int proj = 0x00ff, string extra = null)
        {
            Add(" VALUES (");
            parameterlst(obj, proj);
            if (extra != null)
            {
                Add(",");
                Add(extra);
            }
            Add(")");
            return this;
        }

        public DbSql _SET_(IData obj, int proj = 0x00ff, string extra = null)
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

        public DbSql Put(string name, JNumber value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(value);
            }
            return this;
        }

        public DbSql Put(string name, IDataInput value)
        {
            return this;
        }

        public DbSql Put(string name, bool value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(value ? "TRUE" : "FALSE");
            }
            return this;
        }

        public DbSql Put(string name, short value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(value);
            }
            return this;
        }

        public DbSql Put(string name, int value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(value);
            }
            return this;
        }

        public DbSql Put(string name, long value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(value);
            }
            return this;
        }

        public DbSql Put(string name, double value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(value);
            }
            return this;
        }

        public DbSql Put(string name, decimal value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(value);
            }
            return this;
        }

        public DbSql Put(string name, DateTime value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(value);
            }
            return this;
        }

        public DbSql Put(string name, string value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add('\'');
                Add(value);
                Add('\'');
            }
            return this;
        }

        public DbSql Put(string name, ArraySegment<byte> value)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, short[] value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (value == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add("ARRAY[");
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i > 0) Add(',');
                        Add(value[i]);
                    }
                    Add(']');
                    if (value.Length == 0)
                    {
                        Add("::smallint[]");
                    }
                }
            }
            return this;
        }

        public DbSql Put(string name, int[] value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (value == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add("ARRAY[");
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i > 0) Add(',');
                        Add(value[i]);
                    }
                    Add(']');
                    if (value.Length == 0)
                    {
                        Add("::integer[]");
                    }
                }
            }
            return this;
        }

        public DbSql Put(string name, long[] value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (value == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add("ARRAY[");
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i > 0) Add(',');
                        Add(value[i]);
                    }
                    Add(']');
                    if (value.Length == 0)
                    {
                        Add("::bigint[]");
                    }
                }
            }
            return this;
        }

        public DbSql Put(string name, string[] value)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (value == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add("ARRAY[");
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i > 0) Add(',');
                        Add('\'');
                        Add(value[i]);
                        Add('\'');
                    }
                    Add(']');
                    if (value.Length == 0)
                    {
                        Add("::varchar[]");
                    }
                }
            }
            return this;
        }

        public DbSql Put(string name, Dictionary<string, string> value)
        {
            throw new NotImplementedException();
        }

        public DbSql Put(string name, IData value, int proj = 0x00ff)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (value == null)
                {
                    Add("NULL");
                }
                else
                {
                }
            }
            return this;
        }

        public DbSql Put<D>(string name, D[] value, int proj = 0x00ff) where D : IData
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