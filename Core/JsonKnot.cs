namespace Greatbone.Core
{
    internal struct Knot
    {
        internal int start;

        internal int end;

        // current ordinal
        internal int ordinal;

        // current position
        internal int current;

        // object or array
        internal bool array;

        internal Knot(bool array)
        {
            this.array = array;
            start = end = current = -1;
            ordinal = 0;
        }
    }
}