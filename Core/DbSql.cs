using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A helper used to generate SQL commands.
    ///
    public class DbSql : DynamicContent, IDataOutput<DbSql>
    {
        // contexts
        const sbyte ColumnList = 1, ParameterList = 2, SetList = 3;

        // the putting context
        internal sbyte list;

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
            list = 0;
            ordinal = 0;
        }

        public DbSql _(string str)
        {
            Add(' ');
            Add(str);

            return this;
        }

        public DbSql setlst(IData obj, int proj = 0)
        {
            list = SetList;
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

        public DbSql columnlst(IData obj, int proj = 0)
        {
            list = ColumnList;
            ordinal = 1;
            obj.WriteData(this, proj);
            return this;
        }

        public DbSql parameterlst(IData obj, int proj = 0)
        {
            list = ParameterList;
            ordinal = 1;
            obj.WriteData(this, proj);
            return this;
        }

        public DbSql _(IData obj, int proj = 0)
        {
            Add(" (");
            columnlst(obj, proj);
            Add(")");
            return this;
        }

        public DbSql _VALUES_(IData obj, int proj = 0)
        {
            Add(" VALUES (");
            parameterlst(obj, proj);
            Add(")");
            return this;
        }

        public DbSql _SET_(IData obj, int proj = 0)
        {
            Add(" SET ");
            setlst(obj, proj);
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


        public DbSql PutEnter(bool multi)
        {
            throw new NotImplementedException();
        }

        public DbSql PutExit(bool multi)
        {
            throw new NotImplementedException();
        }

        public DbSql PutRaw(string name, string raw)
        {
            throw new NotImplementedException();
        }

        public DbSql Put(string name, bool v, string Label = null, bool Required = false, Func<bool, string> Options = null)
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

        public DbSql Put(string name, short v, string Label = null, string Placeholder = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false, IDictionary<short, string> Options = null)
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

        public DbSql Put(string name, int v, string Label = null, string Placeholder = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false, IDictionary<int, string> Options = null)
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

        public DbSql Put(string name, long v, string Label = null, string Placeholder = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false, IDictionary<long, string> Options = null)
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

        public DbSql Put(string name, double v, string Label = null, string Placeholder = null, double Max = 0, double Min = 0, double Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbSql Put(string name, decimal v, string Label = null, string Placeholder = null, decimal Max = 0, decimal Min = 0, decimal Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbSql Put(string name, DateTime v, string Label = null, DateTime Max = default(DateTime), DateTime Min = default(DateTime), int Step = 0, bool ReadOnly = false, bool Required = false)
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

        public DbSql Put(string name, string v, string Label = null, string Placeholder = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false, IDictionary<string, string> Options = null)
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

        public DbSql Put(string name, JArr v)
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
                    v.WriteData(this);
                    Add('\'');
                }
            }
            return this;
        }

        public DbSql Put(string name, JObj v)
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
                    v.WriteData(this);
                    Add('\'');
                }
            }
            return this;
        }

        public DbSql Put(string name, short[] v, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false, IDictionary<short, string> Options = null)
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

        public DbSql Put(string name, int[] v, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false, IDictionary<int, string> Options = null)
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

        public DbSql Put(string name, long[] v, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false, IDictionary<long, string> Options = null)
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

        public DbSql Put(string name, string[] v, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false, IDictionary<string, string> Options = null)
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

        public DbSql Put(string name, IDataInput v)
        {
            throw new NotImplementedException();
        }

        public DbSql Put(string name, Dictionary<string, string> v, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
            throw new NotImplementedException();
        }

        public DbSql Put(string name, IData v, int proj = 0, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
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

        public DbSql Put<D>(string name, D[] v, int proj = 0, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false) where D : IData
        {
            Build(name);
            return this;
        }

        public DbSql Put<D>(string name, List<D> v, int proj = 0, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false) where D : IData
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