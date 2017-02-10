using System;

namespace Greatbone.Core
{
    ///
    /// infer form control type by data type and sometimes the name.
    ///
    /// char[] --> password
    /// email --> email
    public struct Ui<V> where V : struct, IComparable<V>
    {
        public string Label { get; set; }

        public bool Pick { get; set; }

        public string Placeholder { get; set; }

        public string Pattern { get; set; }
        
        public bool ReadOnly { get; set; }

        public bool Required { get; set; }

        public V Max { get; set; }

        public V Min { get; set; }

        public int Step { get; set; }
    }
}