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
                        Add('\\'); Add('"');
                    }
                    else if (c == '\\')
                    {
                        Add('\\'); Add('\\');
                    }
                    else if (c == '\n')
                    {
                        Add('\\'); Add('n');
                    }
                    else if (c == '\r')
                    {
                        Add('\\'); Add('r');
                    }
                    else if (c == '\t')
                    {
                        Add('\\'); Add('t');
                    }
                    else
                    {
                        Add(c);
                    }
                }
            }
        }

        //
        // PUT
        //

        public void PutArr(Action a)
        {
            if (counts[level]++ > 0) Add(',');

            counts[++level] = 0; // enter
            Add('[');

            if (a != null) a();

            Add(']');
            level--; // exit
        }

        public void PutArr<P>(P[] arr, byte x = 0xff) where P : IPersist
        {
            Put(null, arr, x);
        }

        public void PutObj(Action a)
        {
            if (counts[level]++ > 0) Add(',');

            counts[++level] = 0; // enter
            Add('{');

            if (a != null) a();

            Add('}');
            level--; // exit
        }

        public void PutObj<P>(P obj, byte x = 0xff) where P : IPersist
        {
            Put(null, obj, x);
        }


        //
        // SINK
        //

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

            Add(v.bigint);
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

            Add('"');
            Add(v);
            Add('"');

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

        public JContent Put(string name, string v, int maxlen = 0)
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
                AddEsc(v);
                Add('"');
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

        public JContent Put<P>(string name, P v, byte x = 0xff) where P : IPersist
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
                counts[++level] = 0; // enter
                Add('{');
                v.Dump(this, x);
                Add('}');
                level--; // exit
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
                counts[++level] = 0; // enter
                Add('{');
                v.Dump(this);
                Add('}');
                level--; // exit
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
                counts[++level] = 0; // enter
                Add('[');
                v.Dump(this);
                Add(']');
                level--; // exit
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
                Add('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add(',');
                    Add(v[i]);
                }
                Add(']');
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
                Add('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add(',');
                    Add(v[i]);
                }
                Add(']');
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
                Add('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add(',');
                    Add(v[i]);
                }
                Add(']');
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
                Add('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add(',');
                    string str = v[i];
                    if (str == null)
                    {
                        Add("null");
                    }
                    else
                    {
                        Add('"');
                        AddEsc(str);
                        Add('"');
                    }
                }
                Add(']');
            }

            return this;
        }


        public JContent Put<P>(string name, P[] v, byte x = 0xff) where P : IPersist
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
                counts[++level] = 0; // enter
                Add('[');
                for (int i = 0; i < v.Length; i++)
                {
                    Put(null, v[i], x);
                }
                Add(']');
                level--; // exit
            }
            return this;
        }

    }

}