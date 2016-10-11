using System;
using System.Text;

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


        //
        // PUT
        //

        public void PutArr(Action a)
        {
            if (counts[level]++ > 0) Write((byte)',');

            counts[++level] = 0;
            Write((byte)'[');

            if (a != null) a();

            Write((byte)']');
            level--;
        }

        public void PutArr<T>(T[] v, ushort x = 0xffff) where T : IPersist
        {
            PutArr(delegate
            {
                for (int i = 0; i < v.Length; i++)
                {
                    PutObj(v[i], x);
                }
            });
        }

        public void PutObj(Action a)
        {
            if (counts[level]++ > 0) Write((byte)',');

            counts[++level] = 0;
            Write((byte)'{');

            if (a != null) a();

            Write((byte)'}');
            level--;
        }

        public void PutObj<T>(T v, ushort x = 0xffff) where T : IPersist
        {
            PutObj(delegate
            {
                v.Save(this, x);
            });
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
                            Write((byte)'\\'); Write((byte)'"');
                        }
                        else if (c == '\\')
                        {
                            Write((byte)'\\'); Write((byte)'\\');
                        }
                        else if (c == '\n')
                        {
                            Write((byte)'\\'); Write((byte)'n');
                        }
                        else if (c == '\r')
                        {
                            Write((byte)'\\'); Write((byte)'r');
                        }
                        else if (c == '\t')
                        {
                            Write((byte)'\\'); Write((byte)'t');
                        }
                        else
                        {
                            Write((byte)c);
                        }
                    }
                    else if (c < 0x800)
                    {
                        // 2 char, 11 bits
                        Write((byte)(0xc0 | (c >> 6)));
                        Write((byte)(0x80 | (c & 0x3f)));
                    }
                    else
                    {
                        // 3 char, 16 bits
                        Write((byte)(0xe0 | ((c >> 12))));
                        Write((byte)(0x80 | ((c >> 6) & 0x3f)));
                        Write((byte)(0x80 | (c & 0x3f)));
                    }
                }
            }
        }

        public JContent Put(string name, bool v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            Add(v ? "true" : "false");

            return this;
        }

        public JContent Put(string name, short v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            Add(v);

            return this;
        }

        public JContent Put(string name, int v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            Add(v);

            return this;
        }

        public JContent Put(string name, long v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            Add(v);

            return this;
        }

        public JContent Put(string name, decimal v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            Add(v);

            return this;
        }

        public JContent Put(string name, Number v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            Add(v.integr);
            if (v.Pt)
            {
                Write((byte)'.');
                Add(v.fract);
            }
            return this;
        }

        public JContent Put(string name, DateTime v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            Add(v);

            return this;
        }

        public JContent Put(string name, char[] v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write((byte)'"');
                Add(v);
                Write((byte)'"');
            }

            return this;
        }

        public JContent Put(string name, string v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write((byte)'"');
                AddEsc(v);
                Write((byte)'"');
            }

            return this;
        }

        public JContent Put(string name, byte[] v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
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

        public JContent Put(string name, ArraySegment<byte> v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
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

        public JContent Put<T>(string name, T v, ushort x = 0xffff) where T : IPersist
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                PutObj(delegate
                {
                    v.Save(this, x);
                });
            }

            return this;
        }

        public JContent Put(string name, JObj v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
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

        public JContent Put(string name, JArr v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
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

        public JContent Put(string name, short[] v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write((byte)'[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write((byte)',');
                    Add(v[i]);
                }
                Write((byte)']');
            }

            return this;
        }

        public JContent Put(string name, int[] v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write((byte)'[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write((byte)',');
                    Add(v[i]);
                }
                Write((byte)']');
            }

            return this;
        }

        public JContent Put(string name, long[] v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write((byte)'[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write((byte)',');
                    Add(v[i]);
                }
                Write((byte)']');
            }

            return this;
        }

        public JContent Put(string name, string[] v)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write((byte)'[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write((byte)',');
                    Add(v[i]);
                }
                Write((byte)']');
            }

            return this;
        }


        public JContent Put<T>(string name, T[] v, ushort x = 0xffff) where T : IPersist
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write((byte)'[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write((byte)',');
                    Put(null, v[i], x); // output a persist object
                }
                Write((byte)']');
            }

            return this;
        }

        public JContent PutNull(string name)
        {
            if (counts[level]++ > 0) Write((byte)',');

            if (name != null)
            {
                Write((byte)'"');
                Add(name);
                Write((byte)'"');
                Write((byte)':');
            }

            Add("null");

            return this;
        }

    }
}