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


        public JContent Put(string name, bool v)
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

        public JContent Put(string name, short v)
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

        public JContent Put(string name, int v)
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

        public JContent Put(string name, long v)
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

        public JContent Put(string name, decimal v)
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

        public JContent Put(string name, Number v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Add('"');
                Add(name);
                Add('"');
                Add(':');
            }

            Add(v.integr);
            if (v.Pt)
            {
                Add('.');
                Add(v.fract);
            }
            return this;
        }

        public JContent Put(string name, DateTime v)
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

        public JContent Put(string name, char[] v)
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
                Add('"');
                Add(v);
                Add('"');
            }

            return this;
        }

        public JContent Put(string name, string v)
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
                Add('"');
                Add(v);
                Add('"');
            }

            return this;
        }

        public JContent Put(string name, byte[] v)
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
                // TODO
            }

            return this;
        }

        public JContent Put<T>(string name, T v, int x = -1) where T : IPersist
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
                level++;
                AddByte((byte)'{');
                v.Save(this, x);
                AddByte((byte)'}');
                level--;
            }

            return this;
        }

        public JContent Put(string name, JObj v)
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
                v.Save(this);
            }

            return this;
        }

        public JContent Put(string name, JArr v)
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
                v.Save(this);
            }
            return this;
        }

        public JContent Put(string name, short[] v)
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
                AddByte((byte)'[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) AddByte((byte)',');
                    Add(v[i]);
                }
                AddByte((byte)']');
            }

            return this;
        }

        public JContent Put(string name, int[] v)
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
                AddByte((byte)'[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) AddByte((byte)',');
                    Add(v[i]);
                }
                AddByte((byte)']');
            }

            return this;
        }

        public JContent Put(string name, long[] v)
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
                AddByte((byte)'[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) AddByte((byte)',');
                    Add(v[i]);
                }
                AddByte((byte)']');
            }

            return this;
        }

        public JContent Put(string name, string[] v)
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
                AddByte((byte)'[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) AddByte((byte)',');
                    Add(v[i]);
                }
                AddByte((byte)']');
            }

            return this;
        }


        public JContent Put<T>(string name, T[] v, int x = -1) where T : IPersist
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
                AddByte((byte)'[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) AddByte((byte)',');
                    Put(null, v[i], x); // output a persist object
                }
                AddByte((byte)']');
            }

            return this;
        }

        public JContent PutNull(string name)
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

    }
}