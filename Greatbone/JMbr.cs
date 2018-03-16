using System;

namespace Greatbone
{
    ///
    /// A JSON member that is either a value, or a property if with name.
    ///
    public struct JMbr : IMappable<string>
    {
        // property name, if not null
        readonly string name;

        // type of the value
        internal readonly JType type;

        // JObj, JArr, string
        internal readonly object refv;

        internal readonly JNumber numv;

        public JMbr(JType jnull, string name = null)
        {
            this.name = name;
            type = jnull;
            refv = null;
            numv = default;
        }

        public JMbr(JObj v, string name = null)
        {
            this.name = name;
            type = JType.Object;
            refv = v;
            numv = default;
        }

        public JMbr(JArr v, string name = null)
        {
            this.name = name;
            type = JType.Array;
            refv = v;
            numv = default;
        }

        public JMbr(string v, string name = null)
        {
            this.name = name;
            type = JType.String;
            refv = v;
            numv = default;
        }

        public JMbr(bool v, string name = null)
        {
            this.name = name;
            type = v ? JType.True : JType.False;
            refv = null;
            numv = default;
        }

        public JMbr(JNumber v, string name = null)
        {
            this.name = name;
            type = JType.Number;
            refv = null;
            numv = v;
        }

        public string Key => name;

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
    }
}