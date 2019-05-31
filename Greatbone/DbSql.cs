using System;

namespace Greatbone
{
    /// <summary>
    /// A specialized string builder for generating SQL commands.
    /// </summary>
    public class DbSql : DynamicContent, ISink
    {
        // contexts
        const sbyte CtxColumnList = 1, CtxParamList = 2, CtxSetList = 3;

        // the putting context
        internal sbyte ctx;

        // used when generating a list
        internal int ordinal;

        internal DbSql(string str) : base(false, 1024)
        {
            Add(str);
        }

        public override string Type { get; set; } = "text/plain";

        internal void Clear()
        {
            count = 0;
            ctx = 0;
            ordinal = 0;
        }

        public DbSql T(short v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }

            return this;
        }

        public DbSql T(int v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }

            return this;
        }

        public DbSql T(string v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }

            return this;
        }

        public DbSql TT(string v, bool cond = true)
        {
            if (cond)
            {
                Add('\'');
                Add(v);
                Add('\'');
            }

            return this;
        }

        public DbSql T(short[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
            {
                if (i > 0) Add(',');
                Add(vals[i]);
            }

            return this;
        }

        public DbSql T(int[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
            {
                if (i > 0) Add(',');
                Add(vals[i]);
            }

            return this;
        }

        public DbSql T(long[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
            {
                if (i > 0) Add(',');
                Add(vals[i]);
            }

            return this;
        }

        public DbSql T(string[] vals)
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


        public DbSql setlst(IData obj, byte proj = 0x0f)
        {
            ctx = CtxSetList;
            ordinal = 1;
            obj.Write(this, proj);
            return this;
        }

        public DbSql collst(IData obj, byte proj = 0x0f)
        {
            ctx = CtxColumnList;
            ordinal = 1;
            obj.Write(this, proj);
            return this;
        }

        public DbSql paramlst(IData obj, byte proj = 0x0f)
        {
            ctx = CtxParamList;
            ordinal = 1;
            obj.Write(this, proj);
            return this;
        }

        public DbSql _(IData obj, byte proj = 0x0f, string extra = null)
        {
            Add(" (");
            collst(obj, proj);
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

        public DbSql _VALUES_(IData obj, byte proj = 0x0f, string extra = null)
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

        public DbSql _SET_(IData obj, byte proj = 0x0f, string extra = null)
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

        public DbSql _IN_(short[] vals)
        {
            Add(" IN (");
            for (int i = 1; i <= vals.Length; i++)
            {
                if (i > 1) Add(',');
                Add('@');
                Add('v');
                Add(i);
            }

            Add(')');
            return this;
        }

        public DbSql _IN_(int[] vals)
        {
            Add(" IN (");
            T(vals);
            Add(')');
            return this;
        }

        public DbSql _IN_(long[] vals)
        {
            Add(" IN (");
            for (int i = 1; i <= vals.Length; i++)
            {
                if (i > 1) Add(',');
                Add('@');
                Add('v');
                Add(i);
            }

            Add(')');
            return this;
        }

        public DbSql _IN_(string[] vals)
        {
            Add(" IN (");
            for (int i = 1; i <= vals.Length; i++)
            {
                if (i > 1) Add(',');
                Add('@');
                Add('v');
                Add(i);
            }

            Add(')');
            return this;
        }

        void Build(string name)
        {
            if (ordinal > 1) Add(", ");

            switch (ctx)
            {
                case CtxColumnList:
                    Add('"');
                    Add(name);
                    Add('"');
                    break;
                case CtxParamList:
                    Add("@");
                    Add(name);
                    break;
                case CtxSetList:
                    Add('"');
                    Add(name);
                    Add('"');
                    Add("=@");
                    Add(name);
                    break;
            }

            ordinal++;
        }

        //
        // SINK
        //

        public void PutNull(string name)
        {
            Build(name);
        }

        public void Put(string name, JNumber v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, bool v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v ? "TRUE" : "FALSE");
            }
        }

        public void Put(string name, char v)
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
        }

        public void Put(string name, short v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, int v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, long v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, double v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, decimal v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, DateTime v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, string v)
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
        }

        public void Put(string name, ArraySegment<byte> v)
        {
            Build(name);
        }

        public void Put(string name, byte[] v)
        {
            Build(name);
        }

        public void Put(string name, short[] v)
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
        }

        public void Put(string name, int[] v)
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
        }

        public void Put(string name, long[] v)
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
        }

        public void Put(string name, string[] v)
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
        }

        public void Put(string name, JObj v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Put(string name, JArr v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Put(string name, IData v, byte proj = 0x0f)
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
            }
        }

        public void Put<D>(string name, D[] v, byte proj = 0x0f) where D : IData
        {
            Build(name);
        }

        public void PutFrom(ISource s)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return new string(charbuf, 0, count);
        }
    }
}