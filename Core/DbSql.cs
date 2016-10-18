using System;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A helper used to generate SQL commands.
    /// </summary>
    ///
    public class DbSql : Text, ISink<DbSql>
    {

        const int InitialCapacity = 1024;

        public const int

            X_SEL = 0x800000,

            X_UPD = 0x400000,

            X_INS = 0x200000;

        // contexts
        const sbyte VALUES = 0, COLUMNS = 1, PARAMS = 2, SETS = 3;

        // the putting context
        internal sbyte ctx;

        // used when generating a list
        internal int ordinal;


        public DbSql() : base(InitialCapacity)
        {
        }

        public DbSql(string str) : base(InitialCapacity)
        {
            Add(str);
        }

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

        public DbSql Sets<T>(T obj, uint x = 0) where T : IPersist
        {
            ctx = SETS;
            ordinal = 1;
            obj.Save(this, x);
            return this;
        }

        public DbSql Columns<T>(T obj, uint x = 0) where T : IPersist
        {
            ctx = COLUMNS;
            ordinal = 1;
            obj.Save(this, x);
            return this;
        }

        public DbSql Params<T>(T obj, uint x = 0) where T : IPersist
        {
            ctx = PARAMS;
            ordinal = 1;
            obj.Save(this, x);
            return this;
        }

        public DbSql Values<T>(T obj, uint x = 0) where T : IPersist
        {
            ctx = VALUES;
            ordinal = 1;
            obj.Save(this, x);
            return this;
        }

        public DbSql WHERE(string cond)
        {
            Add(" WHERE ");
            Add(cond);
            return this;
        }

        void Build(string name)
        {
            if (ordinal > 1) Add(", ");

            switch (ctx)
            {
                case COLUMNS: Add(name); break;
                case PARAMS: Add("@"); Add(name); break;
                case SETS: Add(name); Add("=@"); Add(name); break;
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

        public DbSql Put(string name, string v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add('\''); Add(v); Add('\'');
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
                Add('\''); Add(v); Add('\'');
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

        public DbSql Put<V>(string name, V v, uint x = 0) where V : IPersist
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

        public DbSql Put(string name, JArr v)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, JObj v)
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

        public DbSql Put<V>(string name, V[] v, uint x = 0) where V : IPersist
        {
            Build(name);
            return this;
        }

        public DbSql PutNull(string name)
        {
            Build(name);
            return this;
        }

        //
        // CONVINIENTS
        //

        public DbSql SELECT_WHERE<T>(T obj, string table, uint x = 0, string cond = null) where T : IPersist
        {
            Add("SELECT ");

            Columns(obj, x | X_SEL);

            Add(" FROM "); Add(table);

            if (cond != null)
            {
                Add(" WHERE ");
                Add(cond);
            }

            return this;
        }

        public DbSql INSERT_VALUES<T>(string table, T obj, uint x = 0) where T : IPersist
        {
            Add("INSERT INTO "); Add(table);
            Add(" (");

            Columns(obj, x | X_INS);

            Add(") VALUES (");

            Params(obj, x | X_INS);

            Add(")");

            return this;
        }


        public DbSql UPDATE_SET_WHERE(string table, IPersist obj, uint x = 0, string cond = null)
        {
            Add("UPDATE "); Add(table);
            Add(" SET ");

            Sets(obj, x | X_UPD);

            if (cond != null)
            {
                Add(" WHERE ");
                Add(cond);
            }

            return this;
        }

        public DbSql INSERT_VALUES_UPDATE_SET<T>(string table, string targ, T obj, uint x = 0) where T : IPersist
        {
            INSERT_VALUES(table, obj, x);

            Add(" ON CONFLICT (");
            Add(targ);
            Add(") DO UPDATE SET ");

            Sets(obj, x | X_UPD);

            return this;
        }

        public DbSql DELETE_WHERE(string table, string cond = null)
        {
            Add("DELETE FROM "); Add(table);
            if (cond != null)
            {
                Add(" WHERE ");
                Add(cond);
            }
            return this;
        }

        public DbSql RETURNING(string s)
        {
            Add(" RETURNING ");
            Add(s);
            return this;
        }

    }


}