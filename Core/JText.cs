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
                            Write('\\'); Write('"');
                        }
                        else if (c == '\\')
                        {
                            Write('\\'); Write('\\');
                        }
                        else if (c == '\n')
                        {
                            Write('\\'); Write('n');
                        }
                        else if (c == '\r')
                        {
                            Write('\\'); Write('r');
                        }
                        else if (c == '\t')
                        {
                            Write('\\'); Write('t');
                        }
                        else
                        {
                            Write(c);
                        }
                    }
                    else if (c < 0x800)
                    {
                        // 2 char, 11 bits
                        Write((char)(0xc0 | (c >> 6)));
                        Write((char)(0x80 | (c & 0x3f)));
                    }
                    else
                    {
                        // 3 char, 16 bits
                        Write((char)(0xe0 | ((c >> 12))));
                        Write((char)(0x80 | ((c >> 6) & 0x3f)));
                        Write((char)(0x80 | (c & 0x3f)));
                    }
                }
            }
        }



        //
        // PUT
        //

        public void PutArr(System.Action a)
        {
            if (counts[level]++ > 0) Write(',');

            counts[++level] = 0;
            Write('{');

            if (a != null) a();

            Write('}');
            level--;
        }

        public void PutArr<T>(T[] v, ushort x = 0) where T : IPersist
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
            if (counts[level]++ > 0) Write(',');

            counts[++level] = 0;
            Write('[');

            if (a != null) a();

            Write(']');
            level--;
        }

        public void PutObj<T>(T v, ushort x = 0) where T : IPersist
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
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add("null");

            return this;
        }

        public JText Put(string name, bool v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v ? "true" : "false");

            return this;
        }

        public JText Put(string name, short v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v);

            return this;
        }

        public JText Put(string name, int v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v);

            return this;
        }

        public JText Put(string name, long v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v);

            return this;
        }

        public JText Put(string name, decimal v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v);

            return this;
        }

        public JText Put(string name, Number v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v.integr);
            if (v.Pt)
            {
                Add('.');
                Add(v.fract);
            }
            return this;
        }

        public JText Put(string name, DateTime v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v);

            return this;
        }

        public JText Put(string name, char[] v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Write('"');
            Add(v);
            Write('"');

            return this;
        }

        public JText Put(string name, string v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write('"');
                AddEsc(v);
                Write('"');
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

        public JText Put<F>(string name, F v, ushort x = 0) where F : IPersist
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
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
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
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
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
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
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write(',');
                    Add(v[i]);
                }
                Write(']');
            }

            return this;
        }

        public JText Put(string name, int[] v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write(',');
                    Add(v[i]);
                }
                Write(']');
            }

            return this;
        }

        public JText Put(string name, long[] v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write(',');
                    Add(v[i]);
                }
                Write(']');
            }

            return this;
        }

        public JText Put(string name, string[] v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write(',');

                    string str = v[i];

                    if (str == null)
                    {
                        Add("null");
                    }
                    else
                    {
                        Write('"');
                        AddEsc(str);
                        Write('"');
                    }
                }
                Write(']');
            }

            return this;
        }

        public JText Put<F>(string name, F[] v, ushort x = 0) where F : IPersist
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
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