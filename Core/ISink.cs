using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A data outputing destination.
    /// </summary>
    public interface ISink<R> where R : ISink<R>
    {

        R Put(string name, bool value);

        R Put(string name, short value);

        R Put(string name, int value);

        R Put(string name, long value);

        R Put(string name, decimal value);

        R Put(string name, DateTime value);

        R Put(string name, string value);

        R Put<T>(string name, T value, int x) where T : IPersist;

        R Put<T>(string name, List<T> value, int x) where T : IPersist;

        R Put(string name, byte[] value);

        R Put(string name, Obj value);

        R Put(string name, Arr value);

    }

}