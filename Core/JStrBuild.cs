using System;

namespace Greatbone.Core
{
    public class JStrBuild : ISink<JStrBuild>
    {
        const int InitialCapacity = 1024;

        char[] buffer;

        int count;

        // parsing context for levels

        int[] counts = new int[8];

        int level;

        public JStrBuild(int capacity = InitialCapacity)
        {
            buffer = new char[capacity];
        }

        void Add(char c)
        {

        }

        void Add(string v)
        {

        }

        void Add(int v)
        {

        }

        public void Arr(Action a)
        {

        }

        public void Obj(Action a)
        {


        }


        public JStrBuild Put<T>(T[] v, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }


        public JStrBuild Put(JArr v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild PutNull(string name)
        {
            Add("null");
            return this;
        }

        public JStrBuild Put(string name, bool v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, short v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, int v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, long v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, decimal v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, Number v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, DateTime v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, char[] v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, string v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, byte[] v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put<T>(string name, T v, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, JObj v)
        {
            if (counts[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');

            if (v == null)
            {
                Add("null");
            }
            else
            {
            }

            return this;
        }

        public JStrBuild Put(string name, JArr v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, short[] v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, int[] v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, long[] v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put(string name, string[] v)
        {
            throw new NotImplementedException();
        }

        public JStrBuild Put<T>(string name, T[] v, int x = -1) where T : IPersist
        {
            throw new NotImplementedException();
        }
    }
}