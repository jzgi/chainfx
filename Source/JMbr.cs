using System;

namespace ChainFx
{
    ///
    /// A JSON member that is either a value, or a property if with name.
    ///
    public readonly struct JMbr : IKeyable<string>
    {
        // property name, if not null
        readonly string name;

        // type of the value
        internal readonly JType typ;

        // JObj, JArr, string
        internal readonly object refv;

        internal readonly JNumber numv;

        public JMbr(JType jnull, string name = null)
        {
            this.name = name;
            typ = jnull;
            refv = null;
            numv = default;
        }

        public JMbr(JObj v, string name = null)
        {
            this.name = name;
            typ = JType.Object;
            refv = v;
            numv = default;
        }

        public JMbr(JArr v, string name = null)
        {
            this.name = name;
            typ = JType.Array;
            refv = v;
            numv = default;
        }

        public JMbr(string v, string name = null)
        {
            this.name = name;
            typ = JType.String;
            refv = v;
            numv = default;
        }

        public JMbr(bool v, string name = null)
        {
            this.name = name;
            typ = v ? JType.True : JType.False;
            refv = null;
            numv = default;
        }

        public JMbr(JNumber v, string name = null)
        {
            this.name = name;
            typ = JType.Number;
            refv = null;
            numv = v;
        }

        public string Key => name;

        public bool IsProperty => name != null;

        public bool IsObject => typ == JType.Object;

        public bool IsArray => typ == JType.Array;

        public bool IsString => typ == JType.String;

        public bool IsNumber => typ == JType.Number;

        public bool IsNull => typ == JType.Null;

        public bool IsBoolean => typ is JType.True or JType.False;

        public static implicit operator JObj(JMbr v)
        {
            if (v.typ == JType.Object)
            {
                return (JObj)v.refv;
            }

            return null;
        }

        public static implicit operator JArr(JMbr v)
        {
            if (v.typ == JType.Array)
            {
                return (JArr)v.refv;
            }

            return null;
        }

        public static implicit operator bool(JMbr v)
        {
            return v.typ == JType.True;
        }

        public static implicit operator char(JMbr v)
        {
            if (v.typ == JType.String)
            {
                var str = (string)v.refv;
                return str.Length == 0 ? '\0' : str[0];
            }

            return '\0';
        }

        public static implicit operator short(JMbr v)
        {
            if (v.typ == JType.Number)
            {
                return v.numv.Short;
            }

            return 0;
        }

        public static implicit operator int(JMbr v)
        {
            if (v.typ == JType.Number)
            {
                return v.numv.Int;
            }

            return 0;
        }

        public static implicit operator long(JMbr v)
        {
            if (v.typ == JType.Number)
            {
                return v.numv.Long;
            }

            return 0;
        }

        public static implicit operator double(JMbr v)
        {
            if (v.typ == JType.Number)
            {
                return v.numv.Double;
            }

            return 0;
        }

        public static implicit operator decimal(JMbr v)
        {
            if (v.typ == JType.Number)
            {
                return v.numv.Decimal;
            }

            return 0;
        }

        public static implicit operator JNumber(JMbr v)
        {
            if (v.typ == JType.Number)
            {
                return v.numv;
            }

            return default;
        }

        public static implicit operator DateTime(JMbr v)
        {
            if (v.typ == JType.String)
            {
                string str = (string)v.refv;
                if (TextUtility.TryParseDate(str, out var dt)) return dt;
            }

            return default;
        }

        public static implicit operator TimeSpan(JMbr v)
        {
            if (v.typ == JType.String)
            {
                string str = (string)v.refv;
                if (TextUtility.TryParseTime(str, out var tm)) return tm;
            }

            return default;
        }

        public static implicit operator string(JMbr v)
        {
            if (v.typ == JType.String)
            {
                return (string)v.refv;
            }

            return null;
        }
    }
}