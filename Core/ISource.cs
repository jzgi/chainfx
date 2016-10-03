using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A data intaking source.
    /// </summary>
    public interface ISource
    {
        bool Get(string name, ref bool value);

        bool Get(string name, ref short value);

        bool Get(string name, ref int value);

        bool Get(string name, ref long value);

        bool Get(string name, ref decimal value);

        bool Get(string name, ref DateTime value);

        bool Get(string name, ref string value);

        bool Get<T>(string name, ref T value, int x) where T : IPersist, new();

        bool Get<T>(string name, ref List<T> value, int x) where T : IPersist, new();

        bool Get(string name, ref byte[] value);

        bool Get(string name, ref Obj value);

        bool Get(string name, ref Arr value);

    }

}