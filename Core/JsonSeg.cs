namespace Greatbone.Core
{
    /// <summary>
    /// A parsing segment of a json stream, usually a json data element.
    /// </summary>
    internal struct JsonSeg
    {
        internal int start;

        internal int end;

        // the ordinal in its parent segment
        internal int ordinal;

        // object or array
        internal bool array;

        internal JsonSeg(bool array)
        {
            this.array = array;
            start = end = -1;
            ordinal = 0;
        }
    }
}