namespace Greatbone.Core
{
    internal struct Level
    {
        internal int start;

        internal int end;

        internal int current;

        internal Level(bool c)
        {
            start = end = current = -1;
        }
    }
}