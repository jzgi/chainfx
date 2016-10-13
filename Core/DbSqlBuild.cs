using System;
using System.Text;

namespace Greatbone.Core
{
    public class DbSqlBuild : ISink<DbSqlBuild>
    {
        StringBuilder sb;

        public DbSqlBuild Put(string name, short v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, bool v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, int v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, long v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, Number v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, DateTime v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, decimal v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, string v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, char[] v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, byte[] v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, ArraySegment<byte> v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put<T>(string name, T v, ushort x = 0xffff) where T : IPersist
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, JArr v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, JObj v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, short[] v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, int[] v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, string[] v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put(string name, long[] v)
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild Put<T>(string name, T[] v, ushort x = ushort.MaxValue) where T : IPersist
        {
            sb.Append(name);
            return this;
        }

        public DbSqlBuild PutNull(string name)
        {
            sb.Append(name);
            return this;
        }
    }

}