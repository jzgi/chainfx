namespace Greatbone.Core
{
    /// <summary>
    /// An encountered data struct in left-to-right parsing context.
    /// </summary>
    internal struct Trace
    {
        // object or array
        public bool IsArray;

        public int Start;

        public int End;

        // the ordinal in its parent segment
        public int Ordinal;

        internal Trace(bool array)
        {
            IsArray = array;
            Start = -1;
            End = -1;
            Ordinal = 0;
        }
    }
}