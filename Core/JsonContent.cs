using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// To generate a UTF-8 encoded JSON document. An extension of putting byte array is supported.
    /// </summary>
    public class JsonContent : DynamicContent, IOut
    {
        // starting positions of each level
        readonly int[] nums;

        // current level
        int level;

        public JsonContent(int capacity) : base(capacity)
        {
            nums = new int[8];
            level = -1;
        }

        public override string Type => "application/json";


        public IOut Arr(Action a)
        {
            if (level >= 0 && nums[level] > 0)
            {
                Add(',');
            }

            level++;
            Add('[');

            a?.Invoke();

            Add(']');
            level--;

            return this;
        }

        //
        // READ OBJECT
        //

        public IOut Object(Action a)
        {
            if (level >= 0 && nums[level] > 0)
            {
                Add(',');
            }

            level++;
            Add('{');

            a?.Invoke();

            Add('}');
            level--;

            return this;
        }


        public IOut _(int value)
        {
            if (nums[level] > 0)
            {
                Add(',');
            }

            Add(value);

            return this;
        }

        public IOut Put(string name, short value)
        {
            if (nums[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');
            Add(value);


            return this;
        }

        public IOut Put(string name, int value)
        {
            if (nums[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');
            Add(value);

            return this;
        }

        public IOut Put(string name, long value)
        {
            if (nums[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');
            Add(value);

            return this;
        }

        public IOut Put(string name, decimal value)
        {
            if (nums[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');
            Add(value);

            return this;
        }

        public IOut Put(string name, DateTime value)
        {
            if (nums[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');
            Add(value);

            return this;
        }

        public IOut Put(string name, char[] value)
        {
            if (nums[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');

            if (value == null)
            {
                Add("null");
            }
            else
            {
                Add('"');
                Add(value);
                Add('"');
            }

            return this;
        }

        public IOut Put(string name, string value)
        {
            if (nums[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');

            if (value == null)
            {
                Add("null");
            }
            else
            {
                Add('"');
                Add(value);
                Add('"');
            }

            return this;
        }


        public IOut Put(string name, bool value)
        {
            if (nums[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');

            Add(value ? "true" : "false");

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
            Add('"');
            Add(name);
            Add('"');
            Add(':');

            Add('{');
            foreach (var pair in dict)
            {
                Add('"');
                Add(pair.Key);
                Add('"');
                Add(':');

                //				PutValue(pair.Value);
            }
            Add('}');

            return this;
        }

        public IOut Put<T>(string name, T value) where T : IData
        {
            throw new NotImplementedException();
        }
    }
}