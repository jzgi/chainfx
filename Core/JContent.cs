using System;

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

        public JContent PutArr<T>(T[] lst) where T : IPersist
        {
            if (counts[++level]++ > 0)
            {
                Add(',');
            }

            Add('[');
            for (int i = 0; i < lst.Length; i++)
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


        public JContent Put(bool v)
        {
            if (counts[level]++ > 0) Add(',');
            Add(v ? "true" : "false");
            return this;
        }

        public JContent Put(short v)
        {
            if (counts[level]++ > 0) Add(',');
            Add(v);
            return this;
        }

        public JContent Put(int v)
        {
            if (counts[level]++ > 0) Add(',');
            Add(v);
            return this;
        }

        public JContent Put(long v)
        {
            if (counts[level]++ > 0) Add(',');
            Add(v);
            return this;
        }

        public JContent Put(decimal v)
        {
            if (counts[level]++ > 0) Add(',');
            Add(v);
            return this;
        }

        public JContent Put(DateTime v)
        {
            if (counts[level]++ > 0) Add(',');
            Add(v);
            return this;
        }

        public JContent Put(char[] v)
        {
            if (counts[level]++ > 0) Add(',');
            if (v == null)
            {
                Add("null");
            }
            else
            {
                Add('"');
                Add(v);
                Add('"');
            }
            return this;
        }

        public JContent Put(string v)
        {
            if (counts[level]++ > 0) Add(',');
            if (v == null)
            {
                Add("null");
            }
            else
            {
                Add('"');
                Add(v);
                Add('"');
            }
            return this;
        }

        public JContent Put(byte[] v)
        {
            throw new NotImplementedException();
        }

        public JContent Put<T>(T v) where T : IPersist
        {
            if (counts[level]++ > 0) Add(',');
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

        public JContent Put(JObj v)
        {
            throw new NotImplementedException();
        }

        public JContent Put(JArr v)
        {
            throw new NotImplementedException();
        }

        public JContent Put(short[] v)
        {
            if (counts[level]++ > 0) Add(',');
            // Add(v);
            return this;
        }

        public JContent Put(int[] v)
        {
            if (counts[level]++ > 0) Add(',');
            // Add(v);
            return this;
        }

        public JContent Put(long[] v)
        {
            if (counts[level]++ > 0) Add(',');
            // Add(v);
            return this;
        }

        public JContent Put(string[] v)
        {
            if (counts[level]++ > 0) Add(',');
            // Add(v);
            return this;
        }

        public JContent Put<T>(T[] v) where T : IPersist
        {
            if (counts[level]++ > 0) Add(',');
            if (v == null)
            {
                Add("null");
            }
            else
            {
                PutArr(v);
            }
            return this;
        }

        public JContent PutNull()
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

        public JContent Put(string name, char[] v)
        {
            if (counts[level]++ > 0) Add(',');

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
                Add('"');
                Add(v);
                Add('"');
            }

            return this;
        }

        public JContent Put(string name, string v)
        {
            if (counts[level]++ > 0) Add(',');

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
                Add('"');
                Add(v);
                Add('"');
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

        public JContent Put(string name, short[] v)
        {
            if (counts[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');

            return this;
        }

        public JContent Put(string name, int[] v)
        {
            if (counts[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');

            return this;
        }

        public JContent Put(string name, long[] v)
        {
            if (counts[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');

            return this;
        }

        public JContent Put(string name, string[] v)
        {
            if (counts[level]++ > 0)
            {
                Add(',');
            }

            Add('"');
            Add(name);
            Add('"');
            Add(':');

            return this;
        }


        public JContent Put<T>(string name, T[] v, int x = -1) where T : IPersist
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

        public JContent PutNull(string name)
        {
            if (counts[level]++ > 0)
            {
                Add(',');
            }
            Add("null");
            return this;
        }

    }
}