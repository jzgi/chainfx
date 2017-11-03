using System;

namespace Greatbone.Core
{
    ///
    /// A JSON member that is either a value, or a property if with the name.
    ///
    public struct JMbr : IRollable
    {
        // property name, if not null
        readonly string name;

        // type of the value
        internal readonly JType type;

        // JObj, JArr, string, byte[]
        internal readonly object refv;

        internal readonly JNumber numv;

        public JMbr(string name)
        {
            this.name = name;
            type = JType.Null;
            refv = null;
            numv = default;
        }

        public JMbr(string name, JObj v)
        {
            this.name = name;
            type = JType.Object;
            refv = v;
            numv = default;
        }

        public JMbr(string name, JArr v)
        {
            this.name = name;
            type = JType.Array;
            refv = v;
            numv = default;
        }

        public JMbr(string name, string v)
        {
            this.name = name;
            type = JType.String;
            refv = v;
            numv = default;
        }

        public JMbr(string name, byte[] v)
        {
            this.name = name;
            type = JType.Bytes;
            refv = v;
            numv = default;
        }

        public JMbr(string name, bool v)
        {
            this.name = name;
            type = v ? JType.True : JType.False;
            refv = null;
            numv = default;
        }

        public JMbr(string name, JNumber v)
        {
            this.name = name;
            type = JType.Number;
            refv = null;
            numv = v;
        }

        public string Name => name;

        public bool IsProperty => name != null;

        public static implicit operator JObj(JMbr v)
        {
            if (v.type == JType.Object)
            {
                return (JObj) v.refv;
            }
            return null;
        }

        public static implicit operator JArr(JMbr v)
        {
            if (v.type == JType.Array)
            {
                return (JArr) v.refv;
            }
            return null;
        }

        public static implicit operator bool(JMbr v)
        {
            return v.type == JType.True;
        }

        public static implicit operator short(JMbr v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Short;
            }
            return 0;
        }

        public static implicit operator int(JMbr v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Int;
            }
            return 0;
        }

        public static implicit operator long(JMbr v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Long;
            }
            return 0;
        }

        public static implicit operator double(JMbr v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Double;
            }
            return 0;
        }

        public static implicit operator decimal(JMbr v)
        {
            if (v.type == JType.Number)
            {
                return v.numv.Decimal;
            }
            return 0;
        }

        public static implicit operator JNumber(JMbr v)
        {
            if (v.type == JType.Number)
            {
                return v.numv;
            }
            return default;
        }

        public static implicit operator DateTime(JMbr v)
        {
            if (v.type == JType.String)
            {
                string str = (string) v.refv;
                if (StrUtility.TryParseDate(str, out var dt)) return dt;
            }
            return default;
        }

        public static implicit operator string(JMbr v)
        {
            if (v.type == JType.String)
            {
                return (string) v.refv;
            }
            return null;
        }

        public static implicit operator byte[](JMbr v)
        {
            if (v.type == JType.String)
            {
                return (byte[]) v.refv;
            }
            return null;
        }
    }
}