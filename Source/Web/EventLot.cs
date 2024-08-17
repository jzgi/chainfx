using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ChainFX;

namespace ChainFx.Web;

/// <summary>
/// The output event queue for an org which contains b 
/// </summary>
[DebuggerDisplay("Count = {Count}")]
public class EventLot : IEnumerable<IData>
{
    // initial capacity
    private readonly int capacity;

    private IData[] array;

    // number of elements.
    private int count;

    public DateTime Since { get; internal set; } = DateTime.Now;


    public EventLot(int capacity = 16)
    {
        this.capacity = capacity;
        array = null;
        count = 0;
    }

    public void Add(IData v)
    {
        lock (this)
        {
            // ensure capacity
            if (array == null)
            {
                array = new IData[capacity];
            }
            else
            {
                var len = array.Length;
                if (count >= len)
                {
                    var alloc = new IData[len * 2];
                    Array.Copy(array, 0, alloc, 0, len);
                    array = alloc;
                }
            }

            array[count++] = v;
        }
    }

    public int Count
    {
        get
        {
            lock (this)
            {
                return count;
            }
        }
    }

    public void Dump(JsonBuilder bdr, DateTime now)
    {
        bdr.ARR_();

        lock (this)
        {
            // buying orders
            for (var i = 0; i < count; i++)
            {
                bdr.Put(null, array[i]);
            }

            // clear to zero
            count = 0;

            Since = now;
        }

        bdr._ARR();
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<IData> GetEnumerator()
    {
        throw new NotImplementedException();
    }
}