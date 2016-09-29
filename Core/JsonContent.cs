using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// To generate a UTF-8 encoded JSON document. It includes a binary-value extension to the standard JSON format.
    /// </summary>
    public class JsonContent : DynamicContent, IOut
    {
        // starting positions of each level
        readonly int[] starts;

        // current level
        int level;

        // current position
        int pos;


        public JsonContent(int capacity) : base(capacity)
        {
        }


        public override string Type => "application/json";


        public bool Array(Action a)
        {
            level++;
            Put('[');

            starts[level] = pos;
            a();

            Put(']');
            level--;

            return true;
        }

        public void Write(byte[] value)
        {
            throw new NotImplementedException();
        }

        //
        // READ OBJECT
        //

        public bool Object(Action a)
        {
            level++;
            Put('{');

            starts[level] = pos;
            a();

            Put('}');
            level--;

            return true;
        }


        public IOut _(int value)
        {
            if (starts[level] > 0)
            {
                Put(',');
            }

            Put(value);

            return this;
        }

        public IOut Put(string name, short value)
        {
            if (starts[level] > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);


            return this;
        }

        public IOut Put(string name, int value)
        {
            if (starts[level] > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            return this;
        }

        public IOut Put(string name, decimal value)
        {
            if (starts[level] > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            return this;

        }

        public IOut Put(string name, DateTime value)
        {
            if (starts[level] > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');
            Put(value);

            return this;

        }

        public IOut Put(string name, string value)
        {
            if (starts[level] > 0)
            {
                Put(',');
            }

            Put('"');
            Put(name);
            Put('"');
            Put(':');

            if (value == null)
            {
                Put("null");
            }
            else
            {
                Put('"');
                Put(value);
                Put('"');
            }

            return this;

        }



        public IOut Put(string name, bool value)
        {
            return this;

        }

        public IOut Put(string name, byte[] value)
        {
            throw new NotImplementedException();
        }

        public IOut Put<T>(string name, List<T> list)
        {
            return this;

        }

        public IOut Put<V>(string name, Dictionary<string, V> dict)
        {
            Put('"');
            Put(name);
            Put('"');
            Put(':');

            Put('{');
            foreach (var pair in dict)
            {
                Put('"');
                Put(pair.Key);
                Put('"');
                Put(':');

                //				PutValue(pair.Value);
            }

            Put('}');
            return this;

        }

        public IOut Put(string name, long value)
        {
            throw new NotImplementedException();
        }

        public IOut Put(string name, char[] value)
        {
            throw new NotImplementedException();
        }

        public IOut Put<T>(string name, long value)
        {
            throw new NotImplementedException();
        }

        public IOut Put<T>(string name, T value) where T : IData
        {
            throw new NotImplementedException();
        }
    }
}