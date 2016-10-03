using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A data outputing destination.
    /// </summary>
    public interface IResultSet : ISource
    {
        bool Map<T>(ref T obj, int x = -1) where T : IPersist, new();

        bool Map<T>(ref List<T> lst, int x = -1) where T : IPersist, new();

        bool Get(ref bool value);

        bool Get(ref short value);

        bool Get(ref int value);

        bool Get(ref long value);

        bool Get(ref decimal value);

        bool Get(ref DateTime value);

        bool Get(ref string value);

        bool Get<T>(ref T value, int x) where T : IPersist, new();

        bool Get<T>(ref List<T> value, int x) where T : IPersist, new();

        bool Get(ref byte[] value);

        bool Get(ref Obj value);

        bool Get(ref Arr value);

    }
}