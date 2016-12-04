using System;

namespace Greatbone.Core
{
    /// 
    /// To generate an pack of events.
    /// 
    public class EventsContent : DynamicContent, ISink<EventsContent>
    {
        const int InitialCapacity = 32 * 1024;

        public EventsContent(bool pooled, int capacity = InitialCapacity) : base(true, pooled, capacity)
        {
        }

        public override string CType => "application/events";

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
        // SINK
        //

        public EventsContent PutNull(string name)
        {
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

        public EventsContent Put(string name, bool v)
        {
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

        public EventsContent Put(string name, short v)
        {
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

        public EventsContent Put(string name, int v)
        {
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

        public EventsContent Put(string name, long v)
        {
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

        public EventsContent Put(string name, decimal v)
        {
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

        public EventsContent Put(string name, Number v)
        {
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

        public EventsContent Put(string name, DateTime v)
        {
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

        public EventsContent Put(string name, char[] v)
        {
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

        public EventsContent Put(string name, string v, int maxlen = 0)
        {
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

        public virtual EventsContent Put(string name, byte[] v)
        {
            return this; // ignore ir
        }

        public virtual EventsContent Put(string name, ArraySegment<byte> v)
        {
            return this; // ignore ir
        }

        public EventsContent Put<B>(string name, B v, byte z = 0) where B : IData
        {
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
                Add('{');
                v.Dump(this, z);
                Add('}');
            }

            return this;
        }

        public EventsContent Put(string name, Obj v)
        {
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
                Add('{');
                v.Dump(this);
                Add('}');
            }

            return this;
        }

        public EventsContent Put(string name, Arr v)
        {
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
                v.Dump(this);
                Add(']');
            }
            return this;
        }

        public EventsContent Put(string name, short[] v)
        {
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

        public EventsContent Put(string name, int[] v)
        {
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

        public EventsContent Put(string name, long[] v)
        {
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

        public EventsContent Put(string name, string[] v)
        {
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


        public EventsContent Put<B>(string name, B[] v, byte z = 0) where B : IData
        {
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
                    Put(null, v[i], z);
                }
                Add(']');
            }
            return this;
        }
    }
}