using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// 
    /// To generate a urlencoded byte or char string.
    /// 
    public class FormContent : DynamicContent, IDataOutput<FormContent>
    {
        // hexidecimal characters
        protected static readonly char[] HEX =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        int ordinal = -1;

        public FormContent(bool octal, bool pooled, int capacity = 4092) : base(octal, pooled, capacity)
        {
        }

        public override string Type => "application/x-www-form-urlencoded";

        void AddEsc(string v)
        {
            if (v == null) return;

            for (int i = 0; i < v.Length; i++)
            {
                char c = v[i];
                if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9')
                {
                    Add(c);
                }
                else if (c == '.' || c == '-' || c == '*' || c == '_')
                {
                    Add(c);
                }
                else if (c == ' ')
                {
                    Add('+');
                }
                else
                {
//                    Add('%');
//                    Add(HEX[(c >> 12) & 0x0f]);
//                    Add(HEX[(c >> 8) & 0x0f]);
//                    Add('%');
//                    Add(HEX[(c >> 4) & 0x0f]);
//                    Add(HEX[c & 0x0f]);
                    if (c < 0x80)
                    {
                        // have at most seven bits
                        AddEscByte((byte) c);
                    }
                    else if (c < 0x800)
                    {
                        // 2 char, 11 bits
                        AddEscByte((byte) (0xc0 | (c >> 6)));
                        AddEscByte((byte) (0x80 | (c & 0x3f)));
                    }
                    else
                    {
                        // 3 char, 16 bits
                        AddEscByte((byte) (0xe0 | ((c >> 12))));
                        AddEscByte((byte) (0x80 | ((c >> 6) & 0x3f)));
                        AddEscByte((byte) (0x80 | (c & 0x3f)));
                    }
                }
            }
        }

        void AddEscByte(byte b)
        {
            Add('%');
            Add(HEX[(b >> 4) & 0x0f]);
            Add(HEX[b & 0x0f]);
        }

        //
        // SINK
        //

        public FormContent PutNull(string name)
        {
            return this;
        }

        public FormContent Put(string name, JNumber v)
        {
            return this;
        }

        public FormContent Put(string name, IDataInput v)
        {
            return this;
        }

        public FormContent PutRaw(string name, string raw)
        {
            return this;
        }

        public void Group(string label)
        {
        }

        public void UnGroup()
        {
        }

        public FormContent Put(string name, bool v, Func<bool, string> Opt = null, string Label = null, bool Required = false)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v ? "true" : "false");
            return this;
        }

        public FormContent Put(string name, short v, Opt<short> Opt = null, string Label = null, string Help = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
            return this;
        }

        public FormContent Put(string name, int v, Opt<int> Opt = null, string Label = null, string Help = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
            return this;
        }

        public FormContent Put(string name, long v, Opt<long> Opt = null, string Label = null, string Help = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
            return this;
        }

        public FormContent Put(string name, double v, string Label = null, string Help = null, double Max = 0, double Min = 0, double Step = 0, bool ReadOnly = false, bool Required = false)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
            return this;
        }

        public FormContent Put(string name, decimal v, string Label = null, string Help = null, decimal Max = 0, decimal Min = 0, decimal Step = 0, bool ReadOnly = false, bool Required = false)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
            return this;
        }

        public FormContent Put(string name, DateTime v, string Label = null, DateTime Max = default(DateTime), DateTime Min = default(DateTime), int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(v);
            return this;
        }

        public FormContent Put(string name, string v, Opt<string> Opt = null, string Label = null, string Help = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            AddEsc(v);
            return this;
        }

        public virtual FormContent Put(string name, ArraySegment<byte> v, string Label = null, string Size = null, string Ratio = null, bool Required = false)
        {
            return this; // ignore ir
        }

        public FormContent Put(string name, short[] v, Opt<short> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }
            return this;
        }

        public FormContent Put(string name, int[] v, Opt<int> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }
            return this;
        }

        public FormContent Put(string name, long[] v, Opt<long> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }
            return this;
        }

        public FormContent Put(string name, string[] v, Opt<string> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0) Add(',');
                Add(v[i]);
            }
            return this;
        }

        public FormContent Put(string name, Dictionary<string, string> v, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            throw new NotImplementedException();
        }

        public FormContent Put(string name, IData v, short proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
            return this;
        }

        public FormContent Put<D>(string name, D[] v, short proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool required = false) where D : IData
        {
            return this;
        }
    }
}