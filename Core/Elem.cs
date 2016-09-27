using System;

namespace Greatbone.Core
{

    public enum VT
    {
        String, Array, Object, Null, Bool
    }

    /// <summary>
    /// An element represents either a value or a name/value pair.
    /// </summary>
    public struct Elem : IMember
    {
        // type of value
        VT vt;

        Number numv;

        DateTime dtv;

        // boolean value
        bool boolv;

        // Obj, Arr, string, byte[]
        internal object refv;

        string name;

        public string Key => name;

        public bool IsPair => name != null;

        public static implicit operator Obj(Elem v)
        {
            return (Obj)v.refv;
        }

        public static implicit operator Arr(Elem v)
        {
            return (Arr)v.refv;
        }

        public static implicit operator int(Elem v)
        {
            return 0;
        }

        public static implicit operator string(Elem v)
        {
            return (string)v.refv;
        }
    }
}