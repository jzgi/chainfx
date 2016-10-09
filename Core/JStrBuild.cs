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

        void Add(long v)
        {

        }

        public void PutArr(Action a)
        {

        }

        public void PutObj(Action a)
        {


        }


        public JStrBuild PutNull(string name)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            Add("null");

            return this;
        }

        public JStrBuild Put(string name, bool v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            Add(v ? "true" : "false");

            return this;
        }

        public JStrBuild Put(string name, short v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            Add(v);

            return this;
        }

        public JStrBuild Put(string name, int v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            Add(v);

            return this;
        }

        public JStrBuild Put(string name, long v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            Add(v);

            return this;
        }

        public JStrBuild Put(string name, decimal v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            // Add(v);

            return this;
        }

        public JStrBuild Put(string name, Number v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            // Add(v);

            return this;
        }

        public JStrBuild Put(string name, DateTime v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            // Add(v);

            return this;
        }

        public JStrBuild Put(string name, char[] v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            // Add(v);

            return this;
        }

        public JStrBuild Put(string name, string v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            // Add(v);

            return this;
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
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

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
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            // Add(v);

            return this;
        }

        public JStrBuild Put(string name, short[] v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            // Add(v);

            return this;
        }

        public JStrBuild Put(string name, int[] v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            // Add(v);

            return this;
        }

        public JStrBuild Put(string name, long[] v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            // Add(v);

            return this;
        }

        public JStrBuild Put(string name, string[] v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            // Add(v);

            return this;
        }

        public JStrBuild Put<T>(string name, T[] v, int x = -1) where T : IPersist
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            // Add(v);

            return this;
        }
    }
}