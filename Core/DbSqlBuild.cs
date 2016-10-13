using System;

namespace Greatbone.Core
{
    public class DbSqlBuild : ISink<DbSqlBuild>
    {
        public DbSqlBuild Put(string name, short v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, long v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, Number v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, char[] v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, byte[] v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, JArr v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, int[] v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, string[] v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, long[] v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, short[] v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, JObj v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, string v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, DateTime v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, decimal v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, int v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put(string name, bool v)
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put<T>(string name, T[] v, ushort x = ushort.MaxValue) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild Put<T>(string name, T v, ushort x = ushort.MaxValue) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public DbSqlBuild PutNull(string name)
        {
            throw new NotImplementedException();
        }
    }

}