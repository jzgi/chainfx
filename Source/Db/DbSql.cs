using System;
using System.Globalization;

namespace SkyCloud.Db
{
    /// <summary>
    /// A specialized string builder for generating SQL commands.
    /// </summary>
    public class DbSql : ISink
    {
        static readonly char[] DIGIT =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        // sexagesimal numbers
        static readonly string[] SEX =
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59"
        };

        protected static readonly short[] SHORT =
        {
            1,
            10,
            100,
            1000,
            10000
        };

        protected static readonly int[] INT =
        {
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000,
            1000000000
        };

        protected static readonly long[] LONG =
        {
            1L,
            10L,
            100L,
            1000L,
            10000L,
            100000L,
            1000000L,
            10000000L,
            100000000L,
            1000000000L,
            10000000000L,
            100000000000L,
            1000000000000L,
            10000000000000L,
            100000000000000L,
            1000000000000000L,
            10000000000000000L,
            100000000000000000L,
            1000000000000000000L
        };

        // contexts
        const sbyte CtxColumnList = 1, CtxParamList = 2, CtxSetList = 3;

        // the putting context
        internal sbyte ctx;

        protected char[] charbuf;

        // number of bytes or chars
        protected int count;

        // used when generating a list
        internal int ordinal;

        string alias;

        internal DbSql(string str)
        {
            charbuf = new char[1024];

            Add(str);
        }

        internal void Clear()
        {
            count = 0;
            ctx = 0;
            ordinal = 0;
        }

        public void Add(char c)
        {
            // ensure capacity
            int olen = charbuf.Length; // old length
            if (count >= olen)
            {
                int nlen = olen * 4; // new length
                char[] obuf = charbuf;
                charbuf = new char[nlen];
                Array.Copy(obuf, 0, charbuf, 0, olen);
            }
            charbuf[count++] = c;
        }

        public void Add(string v)
        {
            if (v == null) return;
            int len = v.Length;
            for (int i = 0; i < len; i++)
            {
                Add(v[i]);
            }
        }

        public void Add(short v)
        {
            if (v == 0)
            {
                Add('0');
                return;
            }
            int x = v; // convert to int
            if (v < 0)
            {
                Add('-');
                x = -x;
            }
            bool bgn = false;
            for (int i = SHORT.Length - 1; i > 0; i--)
            {
                int bas = SHORT[i];
                int q = x / bas;
                x = x % bas;
                if (q != 0 || bgn)
                {
                    Add(DIGIT[q]);
                    bgn = true;
                }
            }
            Add(DIGIT[x]); // last reminder
        }

        public void Add(int v)
        {
            if (v >= short.MinValue && v <= short.MaxValue)
            {
                Add((short) v);
                return;
            }

            if (v < 0)
            {
                Add('-');
                v = -v;
            }
            bool bgn = false;
            for (int i = INT.Length - 1; i > 0; i--)
            {
                int bas = INT[i];
                int q = v / bas;
                v = v % bas;
                if (q != 0 || bgn)
                {
                    Add(DIGIT[q]);
                    bgn = true;
                }
            }
            Add(DIGIT[v]); // last reminder
        }

        public void Add(long v)
        {
            if (v >= int.MinValue && v <= int.MaxValue)
            {
                Add((int) v);
                return;
            }

            if (v < 0)
            {
                Add('-');
                v = -v;
            }
            bool bgn = false;
            for (int i = LONG.Length - 1; i > 0; i--)
            {
                long bas = LONG[i];
                long q = v / bas;
                v = v % bas;
                if (q != 0 || bgn)
                {
                    Add(DIGIT[q]);
                    bgn = true;
                }
            }
            Add(DIGIT[v]); // last reminder
        }

        public void Add(double v)
        {
            Add(v.ToString(CultureInfo.CurrentCulture));
        }

        // sign mask
        const int Sign = unchecked((int) 0x80000000);

        ///
        /// This method outputs decimal numbers fastly.
        ///
        public void Add(decimal v)
        {
            int[] bits = decimal.GetBits(v); // get the binary representation
            int low = bits[0], mid = bits[1], hi = bits[2], flags = bits[3];
            int scale = (bits[3] >> 16) & 0x7F;

            if (hi != 0) // if 96 bits, use system api
            {
                Add(v.ToString(CultureInfo.CurrentCulture));
                return;
            }

            // output a minus if negative
            if ((flags & Sign) != 0)
            {
                Add('-');
            }

            if (mid != 0) // if 64 bits
            {
                long x = ((long) mid << 32) + low;
                bool bgn = false;
                for (int i = LONG.Length - 1; i > 0; i--)
                {
                    long bas = LONG[i];
                    long q = x / bas;
                    x = x % bas;
                    if (q != 0 || bgn)
                    {
                        Add(DIGIT[q]);
                        bgn = true;
                    }
                    if (i == scale)
                    {
                        if (!bgn)
                        {
                            Add('0'); // 0.XX
                            bgn = true;
                        }
                        Add('.');
                    }
                }
                Add(DIGIT[x]); // last reminder
            }
            else // 32 bits
            {
                int x = low;
                bool bgn = false;
                for (int i = INT.Length - 1; i > 0; i--)
                {
                    int bas = INT[i];
                    int q = x / bas;
                    x = x % bas;
                    if (q != 0 || bgn)
                    {
                        Add(DIGIT[q]);
                        bgn = true;
                    }
                    if (i == scale)
                    {
                        if (!bgn)
                        {
                            Add('0'); // 0.XX
                            bgn = true;
                        }
                        Add('.');
                    }
                }
                Add(DIGIT[x]); // last reminder
            }

            // to pad extra zeros for monetary output
            if (scale == 0)
            {
                Add(".00");
            }
            else if (scale == 1)
            {
                Add('0');
            }
        }

        public void Add(DateTime v, bool time = false)
        {
            short yr = (short) v.Year;

            // yyyy-mm-dd
            if (yr < 1000) Add('0');
            if (yr < 100) Add('0');
            if (yr < 10) Add('0');
            Add(v.Year);
            Add('-');
            Add(SEX[v.Month]);
            Add('-');
            Add(SEX[v.Day]);

            if (time)
            {
                Add(' '); // a space for separation
                Add(SEX[v.Hour]);
                Add(':');
                Add(SEX[v.Minute]);
                Add(':');
                Add(SEX[v.Second]);
            }
        }

        public DbSql T(short v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }

            return this;
        }

        public DbSql T(int v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }

            return this;
        }

        public DbSql T(string v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }

            return this;
        }

        public DbSql TT(string v, bool cond = true)
        {
            if (cond)
            {
                Add('\'');
                Add(v);
                Add('\'');
            }

            return this;
        }

        public DbSql T(short[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
            {
                if (i > 0) Add(',');
                Add(vals[i]);
            }

            return this;
        }

        public DbSql T(int[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
            {
                if (i > 0) Add(',');
                Add(vals[i]);
            }

            return this;
        }

        public DbSql T(long[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
            {
                if (i > 0) Add(',');
                Add(vals[i]);
            }

            return this;
        }

        public DbSql T(string[] vals)
        {
            Add(" IN (");
            for (int i = 0; i < vals.Length; i++)
            {
                if (i > 0) Add(',');
                Add('\'');
                Add(vals[i]);
                Add('\'');
            }

            Add(')');
            return this;
        }


        public DbSql setlst(IData obj, byte proj = 0x0f)
        {
            ctx = CtxSetList;
            ordinal = 1;
            obj.Write(this, proj);
            return this;
        }

        public DbSql collst(IData obj, byte proj = 0x0f, string alias = null)
        {
            ctx = CtxColumnList;
            ordinal = 1;

            this.alias = alias;

            obj.Write(this, proj);
            // restore non-alias
            this.alias = null;
            return this;
        }

        public DbSql paramlst(IData obj, byte proj = 0x0f)
        {
            ctx = CtxParamList;
            ordinal = 1;
            obj.Write(this, proj);
            return this;
        }

        public DbSql colset(IData obj, byte proj = 0x0f, string extra = null)
        {
            Add('(');
            collst(obj, proj);
            if (extra != null)
            {
                Add(',');
                Add(extra);
            }

            Add(')');
            return this;
        }

        public DbSql _VALUES_(short n)
        {
            Add(" VALUES (");
            for (short i = 1; i <= n; i++)
            {
                if (i > 1)
                {
                    Add(',');
                    Add(' ');
                }

                Add('@');
                Add(i);
            }

            Add(')');
            return this;
        }

        public DbSql _VALUES_(IData obj, byte proj = 0x0f, string extra = null)
        {
            Add(" VALUES (");
            paramlst(obj, proj);
            if (extra != null)
            {
                Add(",");
                Add(extra);
            }

            Add(")");
            return this;
        }

        public DbSql _SET_(IData obj, byte proj = 0x0f, string extra = null)
        {
            Add(" SET ");
            setlst(obj, proj);
            if (extra != null)
            {
                Add(",");
                Add(extra);
            }

            return this;
        }

        public DbSql _IN_(short[] vals, bool literal = false)
        {
            Add(" IN (");
            for (int i = 1; i <= vals.Length; i++)
            {
                if (i > 1) Add(',');
                if (literal)
                {
                    Add(vals[i]);
                }
                else
                {
                    Add('@');
                    Add('v');
                    Add(i);
                }
            }

            Add(')');
            return this;
        }

        public DbSql _IN_(int[] vals, bool literal = false)
        {
            Add(" IN (");
            for (int i = 1; i <= vals.Length; i++)
            {
                if (i > 1) Add(',');
                if (literal)
                {
                    Add(vals[i]);
                }
                else
                {
                    Add('@');
                    Add('v');
                    Add(i);
                }
            }

            Add(')');
            return this;
        }

        public DbSql _IN_(long[] vals, bool literal = false)
        {
            Add(" IN (");
            for (int i = 1; i <= vals.Length; i++)
            {
                if (i > 1) Add(',');
                if (literal)
                {
                    Add(vals[i]);
                }
                else
                {
                    Add('@');
                    Add('v');
                    Add(i);
                }
            }

            Add(')');
            return this;
        }

        public DbSql _IN_(DateTime[] vals)
        {
            Add(" IN (");
            for (int i = 1; i <= vals.Length; i++)
            {
                if (i > 1) Add(',');
                Add('@');
                Add('v');
                Add(i);
            }
            Add(')');
            return this;
        }

        public DbSql _IN_(string[] vals, bool literal = false)
        {
            Add(" IN (");
            for (int i = 1; i <= vals.Length; i++)
            {
                if (i > 1) Add(',');
                if (literal)
                {
                    Add('\'');
                    Add(vals[i]);
                    Add('\'');
                }
                else
                {
                    Add('@');
                    Add('v');
                    Add(i);
                }
            }

            Add(')');
            return this;
        }

        void Build(string name)
        {
            if (ordinal > 1) Add(", ");

            switch (ctx)
            {
                case CtxColumnList:
                    if (alias != null)
                    {
                        Add(alias);
                        Add('.');
                    }
                    Add('"');
                    Add(name);
                    Add('"');
                    break;
                case CtxParamList:
                    Add("@");
                    Add(name);
                    break;
                case CtxSetList:
                    Add('"');
                    Add(name);
                    Add('"');
                    Add("=@");
                    Add(name);
                    break;
            }

            ordinal++;
        }

        //
        // SINK
        //

        public void PutNull(string name)
        {
            Build(name);
        }

        public void Put(string name, JNumber v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, bool v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v ? "TRUE" : "FALSE");
            }
        }

        public void Put(string name, char v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add('\'');
                Add(v);
                Add('\'');
            }
        }

        public void Put(string name, short v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, int v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, long v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, double v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, decimal v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, DateTime v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add(v);
            }
        }

        public void Put(string name, string v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                Add('\'');
                Add(v);
                Add('\'');
            }
        }

        public void Put(string name, ArraySegment<byte> v)
        {
            Build(name);
        }

        public void Put(string name, byte[] v)
        {
            Build(name);
        }

        public void Put(string name, short[] v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (v == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add("ARRAY[");
                    for (int i = 0; i < v.Length; i++)
                    {
                        if (i > 0) Add(',');
                        Add(v[i]);
                    }

                    Add(']');
                    if (v.Length == 0)
                    {
                        Add("::smallint[]");
                    }
                }
            }
        }

        public void Put(string name, int[] v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (v == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add("ARRAY[");
                    for (int i = 0; i < v.Length; i++)
                    {
                        if (i > 0) Add(',');
                        Add(v[i]);
                    }

                    Add(']');
                    if (v.Length == 0)
                    {
                        Add("::integer[]");
                    }
                }
            }
        }

        public void Put(string name, long[] v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (v == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add("ARRAY[");
                    for (int i = 0; i < v.Length; i++)
                    {
                        if (i > 0) Add(',');
                        Add(v[i]);
                    }

                    Add(']');
                    if (v.Length == 0)
                    {
                        Add("::bigint[]");
                    }
                }
            }
        }

        public void Put(string name, string[] v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (v == null)
                {
                    Add("NULL");
                }
                else
                {
                    Add("ARRAY[");
                    for (int i = 0; i < v.Length; i++)
                    {
                        if (i > 0) Add(',');
                        Add('\'');
                        Add(v[i]);
                        Add('\'');
                    }

                    Add(']');
                    if (v.Length == 0)
                    {
                        Add("::varchar[]");
                    }
                }
            }
        }

        public void Put(string name, JObj v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Put(string name, JArr v)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Put(string name, IData v, byte proj = 0x0f)
        {
            if (name != null)
            {
                Build(name);
            }
            else
            {
                if (v == null)
                {
                    Add("NULL");
                }
            }
        }

        public void Put<D>(string name, D[] v, byte proj = 0x0f) where D : IData
        {
            Build(name);
        }

        public void PutFromSource(ISource s)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return new string(charbuf, 0, count);
        }
    }
}