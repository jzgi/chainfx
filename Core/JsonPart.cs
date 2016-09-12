namespace Greatbone.Core
{
    /// <summary>
    /// An encountered component part during parsing json stream, usually a json data element.
    /// </summary>
    internal struct JsonPart
    {
        internal int start;

        internal int end;

        // the ordinal in its parent segment
        internal int ordinal;

        internal int pos;

        // object or array
        internal bool array;

        internal JsonPart(bool array)
        {
            this.array = array;
            start = end = -1;
            ordinal = 0;
            pos = -1;
        }
    }
}