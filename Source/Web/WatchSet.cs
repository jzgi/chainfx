using System;

namespace ChainFX.Web;

/// <summary>
/// A set of watched flags. 
/// </summary>
public class WatchSet
{
    const int MAX = 16;

    readonly Entry[] entries = new Entry[MAX];

    private int toPush;

    private DateTime since = DateTime.Now;

    public void Put(short idx, int num, decimal amt)
    {
        lock (this)
        {
            entries[idx].AddUp(num, amt);

            toPush += num;
        }
    }

    public DateTime Since => since;


    public int Peek(short idx, bool clear = false)
    {
        lock (this)
        {
            var ret = entries[idx].Total;

            if (clear)
            {
                entries[idx].Total = 0;
            }
            return ret;
        }
    }

    public bool HasToPush
    {
        get
        {
            lock (this)
            {
                return toPush > 0;
            }
        }
    }


    internal struct Entry : IKeyable<short>
    {
        internal readonly short typ;

        internal int count;

        internal decimal sum;

        public int Total { get; internal set; }

        internal bool IsEmpty => typ == 0 || count == 0;

        internal bool IsStuffed => typ != 0 && count != 0;

        public Entry(short slot, int num, decimal amount)
        {
            typ = slot;
            count = num;
            sum = amount;

            Total = num;
        }

        internal void AddUp(int num, decimal amount)
        {
            count += num;
            sum += amount;

            Total += num;
        }

        internal void Reset()
        {
            count = 0;
            sum = 0;
        }

        public short Key => typ;
    }
}