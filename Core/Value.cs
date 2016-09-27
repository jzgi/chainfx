using System;

namespace Greatbone.Core
{
    public enum VT
    {
        String, Array, Null, Bool
    }

    public class Value
    {
        VT vt;

        Obj objv;

        Arr arrv;

        Number numv;

        string strv;

        DateTime dtv;

        //boolean value
        bool boolv;

        // null value
        bool nullv;

        public int Int()
        {
            return 0;
        }

        public static implicit operator int(Value value)
        {
            return 0;
        }
    }
}