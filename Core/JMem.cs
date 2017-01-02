using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// A JSON member that is a value or property if with the name.
    ///
    public struct JMem : IRollable
    {
        // property name, if not null
        readonly string name;

        // type of the value
        internal readonly JType type;

        // JObj, JArr, string, byte[]
        internal readonly object refv;

        internal readonly JNumber numv;

        public JMem(string name)
        {
            this.name = name;
            type = JType.Null;
            refv = null;
            numv = default(JNumber);
        }

        public JMem(string name, JObj v)
        {
            this.name = name;
            type = JType.Object;
            refv = v;
            numv = default(JNumber);
        }

        public JMem(string name, JArr v)
        {
            this.name = name;
            type = JType.Array;
            refv = v;
            numv = default(JNumber);
        }

        public JMem(string name, string v)
        {
            this.name = name;
            type = JType.String;
            refv = v;
            numv = default(JNumber);
        }

        public JMem(string name, byte[] v)
        {
            this.name = name;
            type = JType.Bytes;
            refv = v;
            numv = default(JNumber);
        }

        public JMem(string name, bool v)
        {
            this.name = name;
            type = v ? JType.True : JType.False;
            refv = null;
            numv = default(JNumber);
        }

        public JMem(string name, JNumber v)
        {
            this.name = name;
            type = JType.Number;
            refv = null;
            numv = v;
        }

        public string Name => name;

        public bool IsPair => name != null;

        public static implicit operator JObj(JMem v)
        {
            if (v.type == JType.Object)
            {
                return (JObj) v.refv;
            }
            return null;
        }

        public static implicit operator JArr(JMem v)
        {
            if (v.type == JType.Array)
            {
                return (JArr) v.refv;
            }
            return null;
        }

        public static implicit operator bool(JMem v)
        {
            return v.type == JType.True;
        }

        public static implicit operator short(JMem v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Short;
            }
            return 0;
        }

        public static implicit operator int(JMem v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Int;
            }
            return 0;
        }

        public static implicit operator long(JMem v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Long;
            }
            return 0;
        }

        public static implicit operator double(JMem v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Double;
            }
            return 0;
        }

        public static implicit operator decimal(JMem v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Decimal;
            }
            return 0;
        }

        public static implicit operator JNumber(JMem v)
        {
            if (v.type == JType.Number)
            {
                return v.numv;
            }
            return default(JNumber);
        }

        public static implicit operator DateTime(JMem v)
        {
            if (v.type == JType.String)
            {
                string str = (string) v.refv;
                DateTime dt;
                if (StrUtility.TryParseDate(str, out dt)) return dt;
            }
            return default(DateTime);
        }

        public static implicit operator NpgsqlPoint(JMem v)
        {
            if (v.type == JType.String)
            {
                string str = (string) v.refv;
                if (str != null)
                {
                    int comma = str.IndexOf(',');
                    if (comma != -1)
                    {
                        string xstr = str.Substring(comma);
                        string ystr = str.Substring(comma + 1);
                        double x, y;
                        if (double.TryParse(xstr, out x) && double.TryParse(ystr, out y))
                        {
                            return new NpgsqlPoint(x, y);
                        }
                    }
                }
            }
            return default(NpgsqlPoint);
        }

        public static implicit operator char[](JMem v)
        {
            if (v.type == JType.String)
            {
                string str = (string) v.refv;
                return str.ToCharArray();
            }
            return null;
        }

        public static implicit operator string(JMem v)
        {
            if (v.type == JType.String)
            {
                return (string) v.refv;
            }
            return null;
        }

        public static implicit operator byte[](JMem v)
        {
            if (v.type == JType.String)
            {
                return (byte[]) v.refv;
            }
            return null;
        }
    }
}