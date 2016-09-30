using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A data outputing destination.
    /// </summary>
    public interface IDataOut<R> where R : IDataOut<R> 
    {

        R Put(string name, bool value);

        R Put(string name, short value);

        R Put(string name, int value);

        R Put(string name, long value);

        R Put(string name, decimal value);

        R Put(string name, DateTime value);

        R Put(string name, char[] value);

        R Put(string name, string value);

        R Put<T>(string name, T value) where T : IData;

        R Put(string name, byte[] value);

        R Put<T>(string name, List<T> value);

        R Put<T>(string name, Dictionary<string, T> value);
    }
}