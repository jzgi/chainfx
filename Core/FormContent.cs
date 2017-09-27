using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// To generate a urlencoded byte or char string.
    /// </summary>
    public class FormContent : DynamicContent, IDataOutput<FormContent>
    {
        // hexidecimal characters
        protected static readonly char[] HEX =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        int ordinal = -1;

        public FormContent(bool octet, int capacity = 4092) : base(octet, capacity)
        {
        }

        public override string Type => "application/x-www-form-urlencoded";

        void AddEsc(string v)
        {
            if (v == null) return;

            for (int i = 0; i < v.Length; i++)
            {
                char c = v[i];
                if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9') // alphabetic and decimal digits
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

        public FormContent Put(string name, JNumber value)
        {
            return this;
        }

        public FormContent Put(string name, IDataInput value)
        {
            return this;
        }

        public FormContent Put(string name, bool value)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(value ? "true" : "false");
            return this;
        }

        public FormContent Put(string name, short value)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(value);
            return this;
        }

        public FormContent Put(string name, int value)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(value);
            return this;
        }

        public FormContent Put(string name, long value)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(value);
            return this;
        }

        public FormContent Put(string name, double value)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(value);
            return this;
        }

        public FormContent Put(string name, decimal value)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(value);
            return this;
        }

        public FormContent Put(string name, DateTime value)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            Add(value);
            return this;
        }

        public FormContent Put(string name, string value)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            AddEsc(value);
            return this;
        }

        public virtual FormContent Put(string name, ArraySegment<byte> value)
        {
            return this; // ignore ir
        }

        public FormContent Put(string name, short[] value)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) Add(',');
                Add(value[i]);
            }
            return this;
        }

        public FormContent Put(string name, int[] value)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) Add(',');
                Add(value[i]);
            }
            return this;
        }

        public FormContent Put(string name, long[] value)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) Add(',');
                Add(value[i]);
            }
            return this;
        }

        public FormContent Put(string name, string[] value)
        {
            ordinal++;

            if (ordinal > 0)
            {
                Add('&');
            }
            Add(name);
            Add('=');
            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) Add(',');
                Add(value[i]);
            }
            return this;
        }

        public FormContent Put(string name, Dictionary<string, string> value)
        {
            throw new NotImplementedException();
        }

        public FormContent Put(string name, IData value, int proj = 0x00ff)
        {
            return this;
        }

        public FormContent Put<D>(string name, D[] value, int proj = 0x00ff) where D : IData
        {
            return this;
        }
    }
}