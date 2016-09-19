using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A reader that reads serialized content.
    /// </summary>
    public interface ISerialReader
    {
        //
        // knot reading

        bool ReadArray(Action a);

        bool ReadObject(Action a);

        //
        // value reading

        bool Read(out bool value);

        bool Read(out short value);

        bool Read(out int value);

        bool Read(out long value);

        bool Read(out decimal value);

        bool Read(out DateTime value);

        bool Read(out char[] value);

        bool Read(out string value);

        bool Read<T>(out T value) where T : ISerial, new();

        bool Read<T>(out T[] value);

        bool Read<T>(out List<T> value);

        bool Read<T>(out Dictionary<string, T> value);

        //
        // property reading

        bool Read(string name, out bool value);

        bool Read(string name, out short value);

        bool Read(string name, out int value);

        bool Read(string name, out long value);

        bool Read(string name, out decimal value);

        bool Read(string name, out DateTime value);

        bool Read(string name, out char[] value);

        bool Read(string name, out string value);

        bool Read<T>(string name, out T value) where T : ISerial, new();

        bool Read<T>(string name, out T[] value);

        bool Read<T>(string name, out List<T> value);

        bool Read<T>(string name, out Dictionary<string, T> value);
    }
}