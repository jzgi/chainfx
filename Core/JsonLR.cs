namespace Greatbone.Core
{
    /// <summary>
    /// An encountered data struct in left-to-right parsing context.
    /// </summary>
    internal struct JsonLR
    {
        internal int start;

        internal int end;

        // the ordinal in its parent segment
        internal int ordinal;

        internal int pos;

        // object or array
        internal bool array;

        internal JsonLR(bool array)
        {
            this.array = array;
            start = end = -1;
            ordinal = 0;
            pos = -1;
        }
    }
}