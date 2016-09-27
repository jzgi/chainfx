using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary> <summary>
    /// A serial content writer.
    /// </summary>
    public interface IOutput
    {

        //
        // knot writing

        void Arr(Action a);

        void Obj(Action a);

        void Put(string name, bool value);

        void Put(string name, short value);

        void Put(string name, int value);

        void Put(string name, long value);

        void Put(string name, decimal value);

        void Put(string name, DateTime value);

        void Put(string name, char[] value);

        void Put(string name, string value);

        void Put<T>(string name, T value);

        void Put(string name, byte[] value);

        void Put<T>(string name, List<T> value);

        void Put<T>(string name, Dictionary<string, T> value);
    }
}