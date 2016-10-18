using System;

namespace Greatbone.Core
{
    public class JText : Text, ISink<JText>
    {

        const int InitialCapacity = 1024;

        // parsing context for levels
        int[] counts;

        int level;

        public JText(int capacity = InitialCapacity) : base(capacity)
        {
            counts = new int[8];
            level = 0;
        }

        void AddEsc(string v)
        {
            if (v != null)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    char c = v[i];
                    if (c < 0x80)
                    {
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
                    else if (c < 0x800)
                    {
                        // 2 char, 11 bits
                        Add((char)(0xc0 | (c >> 6)));
                        Add((char)(0x80 | (c & 0x3f)));
                    }
                    else
                    {
                        // 3 char, 16 bits
                        Add((char)(0xe0 | ((c >> 12))));
                        Add((char)(0x80 | ((c >> 6) & 0x3f)));
                        Add((char)(0x80 | (c & 0x3f)));
                    }
                }
            }
        }



        //
        // PUT
        //

        public void PutArr(System.Action a)
        {
            if (counts[level]++ > 0) Add(',');

            counts[++level] = 0;
            Add('{');

            if (a != null) a();

            Add('}');
            level--;
        }

        public void PutArr<T>(T[] v, uint x = 0) where T : IPersist
        {
            PutArr(delegate
            {
                for (int i = 0; i < v.Length; i++)
                {
                    T obj = v[i];
                    if (obj == null)
                    {
                        PutNull(null);
                    }
                    else
                    {
                        PutObj(obj, x);
                    }
                }
            });
        }

        public void PutObj(System.Action a)
        {
            if (counts[level]++ > 0) Add(',');

            counts[++level] = 0;
            Add('[');

            if (a != null) a();

            Add(']');
            level--;
        }

        public void PutObj<T>(T v, uint x = 0) where T : IPersist
        {
            PutObj(delegate
            {
                v.Save(this, x);
            });
        }


        //
        // SINK
        //

        public JText PutNull(string name)
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

        public JText Put(string name, bool v)
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

        public JText Put(string name, short v)
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

        public JText Put(string name, int v)
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

        public JText Put(string name, long v)
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

        public JText Put(string name, decimal v)
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

        public JText Put(string name, Number v)
        {
            if (counts[level]++ > 0) Add((int)',');

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
                Add((int)'.');
                Add(v.fract);
            }
            return this;
        }

        public JText Put(string name, DateTime v)
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

        public JText Put(string name, char[] v)
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

        public JText Put(string name, string v)
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

        public JText Put(string name, byte[] v)
        {
            throw new NotImplementedException();
        }

        public JText Put(string name, ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public JText Put<V>(string name, V v, uint x = 0) where V : IPersist
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
                PutObj(v, x);
            }

            return this;
        }

        public JText Put(string name, JObj v)
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
                PutObj(delegate
                {
                    v.Save(this);
                });
            }

            return this;
        }

        public JText Put(string name, JArr v)
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
                PutArr(delegate
                {
                    v.Save(this);
                });
            }

            return this;
        }

        public JText Put(string name, short[] v)
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

        public JText Put(string name, int[] v)
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

        public JText Put(string name, long[] v)
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

        public JText Put(string name, string[] v)
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

        public JText Put<V>(string name, V[] v, uint x = 0) where V : IPersist
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
                PutArr(v, x);
            }

            return this;
        }

        public override string ToString()
        {
            return new string(buffer, 0, count);
        }
    }
}