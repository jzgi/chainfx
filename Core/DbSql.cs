using System;

namespace Greatbone.Core
{
    public class DbSql : Text, ISink<DbSql>
    {

        const int InitialCapacity = 512;

        // clauses
        const sbyte ColumnList = 1, ValueList = 2, SetList = 3;

        int ordinal;

        // indication
        internal sbyte clause;


        public DbSql(int capacity = InitialCapacity) : base(capacity)
        {
            ordinal = 0;
        }


        public static DbSql SELECT(IPersist obj, string table, ushort x = 0)
        {
            DbSql sql = new DbSql();

            sql.Add("SELECT ");

            sql.clause = ColumnList;
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
            obj.Save(sql, (ushort)(x | UShortkUtility.INS));

            sql.Add(") VALUES (");

            sql.clause = ValueList;
            obj.Save(sql, (ushort)(x | UShortkUtility.INS));

            sql.Add(") ");

            return sql;
        }


        public static DbSql UPDATE(string table, IPersist obj, ushort x = 0)
        {
            DbSql sql = new DbSql();

            sql.Add("UPDATE ");
            sql.Add(table);
            sql.Add(" SET ");

            sql.clause = ColumnList;
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
            return this;
        }

        public DbSql Put(string name, short v)
        {
            if (ordinal++ > 1) Write(',');

            if (clause == ColumnList)
            {
                Add(name);
            }
            else
            {
                Add(name);
            }

            return this;
        }

        public DbSql Put(string name, bool v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, int v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, long v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, Number v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, DateTime v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, decimal v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, string v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, char[] v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, byte[] v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, ArraySegment<byte> v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put<F>(string name, F v, ushort x = 0) where F : IPersist
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, JArr v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, JObj v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, short[] v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, int[] v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, string[] v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put(string name, long[] v)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql Put<F>(string name, F[] v, ushort x = ushort.MaxValue) where F : IPersist
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }

        public DbSql PutNull(string name)
        {
            if (ordinal++ > 0) Write(',');

            Add(name);
            return this;
        }
    }

}