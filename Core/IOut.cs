using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A data outputing destination.
    /// </summary>
    public interface IOut
    {

        IOut Put(string name, bool value);

        IOut Put(string name, short value);

        IOut Put(string name, int value);

        IOut Put(string name, long value);

        IOut Put(string name, decimal value);

        IOut Put(string name, DateTime value);

        IOut Put(string name, char[] value);

        IOut Put(string name, string value);

        IOut Put<T>(string name, T value) where T : IData;

        IOut Put(string name, byte[] value);

        IOut Put<T>(string name, List<T> value);

        IOut Put<T>(string name, Dictionary<string, T> value);
    }
}