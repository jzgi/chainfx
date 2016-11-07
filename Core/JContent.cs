using System;

namespace Greatbone.Core
{

    /// <summary>
    /// To generate a UTF-8 encoded JSON document. An extension of putting byte array is supported.
    /// </summary>
    public class JContent : DynamicContent, ISink<JContent>
    {
        const int InitialCapacity = 4 * 1024;

        // starting positions of each level
        readonly int[] counts;

        // current level
        int level;

        public JContent(bool raw, bool pooled, int capacity = InitialCapacity) : base(raw, pooled, capacity)
        {
            counts = new int[8];
            level = 0;
        }

        public override string Type => "application/json";

        void AddEsc(string v)
        {
            if (v != null)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    char c = v[i];
                    if (c == '\"')
                    {
                        AddChar('\\'); AddChar('"');
                    }
                    else if (c == '\\')
                    {
                        AddChar('\\'); AddChar('\\');
                    }
                    else if (c == '\n')
                    {
                        AddChar('\\'); AddChar('n');
                    }
                    else if (c == '\r')
                    {
                        AddChar('\\'); AddChar('r');
                    }
                    else if (c == '\t')
                    {
                        AddChar('\\'); AddChar('t');
                    }
                    else
                    {
                        AddChar(c);
                    }
                }
            }
        }

        //
        // PUT
        //

        public void PutArr(Action a)
        {
            if (counts[level]++ > 0) AddChar(',');

            counts[++level] = 0; // enter
            AddChar('[');

            if (a != null) a();

            AddChar(']');
            level--; // exit
        }

        public void PutArr<P>(P[] arr, byte z = 0) where P : IPersist
        {
            Put(null, arr, z);
        }

        public void PutObj(Action a)
        {
            if (counts[level]++ > 0) AddChar(',');

            counts[++level] = 0; // enter
            AddChar('{');

            if (a != null) a();

            AddChar('}');
            level--; // exit
        }

        public void PutObj<P>(P obj, byte z = 0) where P : IPersist
        {
            Put(null, obj, z);
        }


        //
        // SINK
        //

        public JContent PutNull(string name)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            Add("null");

            return this;
        }

        public JContent Put(string name, bool v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            Add(v ? "true" : "false");

            return this;
        }

        public JContent Put(string name, short v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            Add(v);

            return this;
        }

        public JContent Put(string name, int v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            Add(v);

            return this;
        }

        public JContent Put(string name, long v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            Add(v);

            return this;
        }

        public JContent Put(string name, decimal v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            Add(v);

            return this;
        }

        public JContent Put(string name, Number v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            Add(v.bigint);
            if (v.Pt)
            {
                AddChar('.');
                Add(v.fract);
            }
            return this;
        }

        public JContent Put(string name, DateTime v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            AddChar('"');
            Add(v);
            AddChar('"');

            return this;
        }

        public JContent Put(string name, char[] v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                AddChar('"');
                Add(v);
                AddChar('"');
            }

            return this;
        }

        public JContent Put(string name, string v, int maxlen = 0)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                AddChar('"');
                AddEsc(v);
                AddChar('"');
            }

            return this;
        }

        public virtual JContent Put(string name, byte[] v)
        {
            return this; // ignore ir
        }

        public virtual JContent Put(string name, ArraySegment<byte> v)
        {
            return this; // ignore ir
        }

        public JContent Put<P>(string name, P v, byte z = 0) where P : IPersist
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter
                AddChar('{');
                v.Dump(this, z);
                AddChar('}');
                level--; // exit
            }

            return this;
        }

        public JContent Put(string name, JObj v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter
                AddChar('{');
                v.Dump(this);
                AddChar('}');
                level--; // exit
            }

            return this;
        }

        public JContent Put(string name, JArr v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter
                AddChar('[');
                v.Dump(this);
                AddChar(']');
                level--; // exit
            }
            return this;
        }

        public JContent Put(string name, short[] v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                AddChar('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) AddChar(',');
                    Add(v[i]);
                }
                AddChar(']');
            }

            return this;
        }

        public JContent Put(string name, int[] v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                AddChar('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) AddChar(',');
                    Add(v[i]);
                }
                AddChar(']');
            }

            return this;
        }

        public JContent Put(string name, long[] v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                AddChar('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) AddChar(',');
                    Add(v[i]);
                }
                AddChar(']');
            }

            return this;
        }

        public JContent Put(string name, string[] v)
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                AddChar('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) AddChar(',');
                    string str = v[i];
                    if (str == null)
                    {
                        Add("null");
                    }
                    else
                    {
                        AddChar('"');
                        AddEsc(str);
                        AddChar('"');
                    }
                }
                AddChar(']');
            }

            return this;
        }


        public JContent Put<P>(string name, P[] v, byte z = 0) where P : IPersist
        {
            if (counts[level]++ > 0) AddChar(',');

            if (name != null)
            {
                AddChar('"');
                Add(name);
                AddChar('"');
                AddChar(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                counts[++level] = 0; // enter
                AddChar('[');
                for (int i = 0; i < v.Length; i++)
                {
                    Put(null, v[i], z);
                }
                AddChar(']');
                level--; // exit
            }
            return this;
        }

    }

}