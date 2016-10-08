using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// To generate a UTF-8 encoded JSON document. An extension of putting byte array is supported.
    /// </summary>
    public class JContent : DynamicContent, ISink<JContent>
    {
        const int InitialCapacity = 16 * 1024;

        // starting positions of each level
        readonly int[] counts;

        // current level
        int level;

        public JContent(int capacity = InitialCapacity) : base(capacity)
        {
            counts = new int[8];
            level = -1;
        }

        public override string Type => "application/json";


        public JContent PutArr(Action a)
        {
            if (counts[++level]++ > 0)
            {
                Add(',');
            }

            Add('[');
            a?.Invoke();
            Add(']');

            counts[level--] = 0;
            return this;
        }

        public JContent PutArr<T>(List<T> lst) where T : IPersist
        {
            if (counts[++level]++ > 0)
            {
                Add(',');
            }

            Add('[');
            for (int i = 0; i < lst.Count; i++)
            {
                Put(lst[i]);
            }
            Add(']');

            counts[level--] = 0;

            return this;
        }

        public void PutObj(Action a)
        {
            if (counts[++level]++ > 0)
            {
                Add(',');
            }

            Add('{');
            a?.Invoke();
            Add('}');

            counts[level--] = 0;
        }

        public void PutObj<T>(T obj) where T : IPersist
        {
            if (counts[++level]++ > 0)
            {
                Add(',');
            }

            Add('{');
            obj.Save(this);
            Add('}');

            counts[level--] = 0;
        }


        public void Put(int value)
        {
            if (counts[level]++ > 0)
            {
                Add(',');
            }

            Add(value);
        }

        public JContent Put<T>(T value) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public JContent PutNull(string name)
        {
            if (counts[level]++ > 0)
            {
                Add(',');
            }
            Add("null");
            return this;
        }


        public JContent Put(string name, bool value)
        {
            if (counts[level]++ > 0)
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

        public JContent Put(string name, short value)
        {
            if (counts[level]++ > 0)
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

        public JContent Put(string name, int value)
        {
            if (counts[level]++ > 0)
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

        public JContent Put(string name, long value)
        {
            if (counts[level]++ > 0)
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

        public JContent Put(string name, decimal value)
        {
            if (counts[level]++ > 0)
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

        public JContent Put(string name, DateTime value)
        {
            if (counts[level]++ > 0)
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

        public JContent Put(string name, string value)
        {
            if (counts[level]++ > 0)
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

        public JContent Put<T>(string name, T v, int x = -1) where T : IPersist
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
                PutObj(v);
            }

            return this;
        }

        public JContent Put<T>(string name, List<T> v, int x = -1) where T : IPersist
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

        public JContent Put(string name, byte[] v)
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

        public JContent Put(string name, JObj v)
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

        public JContent Put(string name, JArr v)
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
    }
}