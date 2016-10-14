using System;

namespace Greatbone.Core
{
    public class DbSql : Text, ISink<DbSql>
    {

        const int InitialCapacity = 512;

        // clauses
        const sbyte ColumnList = 1, ValueList = 2, SetList = 3;

        internal int ordinal;

        // indication
        internal sbyte clause;


        public DbSql(int capacity = InitialCapacity) : base(capacity)
        {
            ordinal = 1;
        }


        public static DbSql SELECT_FROM(IPersist obj, string table, ushort x = 0)
        {
            DbSql sql = new DbSql();

            sql.Add("SELECT ");

            sql.clause = ColumnList;
            sql.ordinal = 1;
            obj.Save(sql, (ushort)(x | UShortkUtility.SEL));

            sql.Add(" FROM ");
            sql.Add(table);

            return sql;
        }

        public static DbSql INSERT_INTO(string table, IPersist obj, ushort x = 0)
        {
            DbSql sql = new DbSql();

            sql.Add("INSERT INTO ");
            sql.Add(table);
            sql.Add(" (");

            sql.clause = ColumnList;
            sql.ordinal = 1;
            obj.Save(sql, (ushort)(x | UShortkUtility.INS));

            sql.Add(") VALUES (");

            sql.clause = ValueList;
            sql.ordinal = 1;
            obj.Save(sql, (ushort)(x | UShortkUtility.INS));

            sql.Add(")");

            return sql;
        }


        public static DbSql UPDATE_SET(string table, IPersist obj, ushort x = 0)
        {
            DbSql sql = new DbSql();

            sql.Add("UPDATE ");
            sql.Add(table);
            sql.Add(" SET ");

            sql.clause = ColumnList;
            sql.ordinal = 1;
            obj.Save(sql, (ushort)(x | UShortkUtility.UPD)); // column list

            return sql;
        }

        public DbSql WHERE(string cond)
        {
            Add(" WHERE ");
            Add(cond);
            
            return this;
        }

        public DbSql RETURNING(string output)
        {
            Add(" RETURNING ");
            Add(output);

            return this;
        }


        void Build(string name)
        {
            if (ordinal > 1) Add(", ");

            switch (clause)
            {
                case ColumnList: Add(name); break;
                case ValueList: Add("@"); Add(name); break;
                case SetList: Add(name); Add("=@"); Add(name); break;
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

        public DbSql Put<F>(string name, F v, ushort x = 0) where F : IPersist
        {
            Build(name);
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
            Build(name);
            return this;
        }

        public DbSql Put(string name, int[] v)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, string[] v)
        {
            Build(name);
            return this;
        }

        public DbSql Put(string name, long[] v)
        {
            Build(name);
            return this;
        }

        public DbSql Put<F>(string name, F[] v, ushort x = ushort.MaxValue) where F : IPersist
        {
            Build(name);
            return this;
        }

        public DbSql PutNull(string name)
        {
            Build(name);
            return this;
        }
    }

}