using System;

namespace Greatbone.Core
{
    public class DbSql : Text, ISink<DbSql>
    {

        const int InitialCapacity = 1024;

        // clauses
        const sbyte Columns = 1, Parameters = 2, Sets = 3;

        internal int ordinal;

        // indication
        internal sbyte ctx;


        public DbSql(string str) : base(InitialCapacity)
        {
            ordinal = 1;
            Add(str);
        }

        public DbSql _(string str)
        {
            Add(' ');
            Add(str);

            return this;
        }

        void Build(string name)
        {
            if (ordinal > 1) Add(", ");

            switch (ctx)
            {
                case Columns: Add(name); break;
                case Parameters: Add("@"); Add(name); break;
                case Sets: Add(name); Add("=@"); Add(name); break;
            }

            ordinal++;
        }

        public DbSql Put(string name, bool v)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, short v)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, int v)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, long v)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, Number v)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, DateTime v)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, decimal v)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, string v)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, char[] v)
        {
            Build(name);
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

        public DbSql Put<V>(string name, V v, ushort x = 0) where V : IPersist
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

        public DbSql Put<V>(string name, V[] v, ushort x = 0) where V : IPersist
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

        public static DbSql SELECT_FROM<T>(T obj, string table, ushort x = 0) where T : IPersist
        {
            DbSql sql = new DbSql("SELECT ");
            sql.Put(obj, (ushort)(x | UShortkUtility.X_SEL))._("FROM")._(table);
            return sql;
        }

        public static DbSql INSERT_INTO<T>(string table, T obj, ushort x = 0) where T : IPersist
        {
            DbSql sql = new DbSql("INSERT INTO ");

            sql.Add(table);
            sql.Add(" (");

            sql.ctx = Columns;
            sql.ordinal = 1;
            obj.Save(sql, x.OrINS());

            sql.Add(") VALUES (");

            sql.ctx = Parameters;
            sql.ordinal = 1;
            obj.Save(sql, x.OrINS());

            sql.Add(")");

            return sql;
        }


        public static DbSql UPDATE_SET(string table, IPersist obj, ushort x = 0)
        {
            DbSql sql = new DbSql("UPDATE ");

            sql.Add(table);
            sql.Add(" SET ");

            sql.ctx = Columns;
            sql.ordinal = 1;
            obj.Save(sql, x.OrUPD()); // column list

            return sql;
        }

        public static DbSql INSERT_INTO_UPDATE_SET<T>(string table, T obj, ushort x = 0) where T : IPersist
        {
            DbSql sql = new DbSql("INSERT INTO ");

            sql.Add(table);
            sql.Add(" (");

            sql.ctx = Columns;
            sql.ordinal = 1;
            obj.Save(sql, x.OrINS());

            sql.Add(") VALUES (");

            sql.ctx = Parameters;
            sql.ordinal = 1;
            obj.Save(sql, x.OrINS());

            sql.Add(")");

            return sql;
        }

    }


}