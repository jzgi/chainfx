using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A reader that reads serialized content.
    /// </summary>
    public interface ISerialReader
    {
        bool ReadArray(Action a);

        bool ReadObject(Action a);

        bool Read(string name, ref bool value);

        bool Read(string name, ref short value);

        bool Read(string name, ref int value);

        bool Read(string name, ref decimal value);

        bool Read(string name, ref DateTime value);

        bool Read(string name, ref string value);

        bool Read<T>(string name, ref T value) where T : ISerial, new();

        bool Read(string name, ref List<string> value);

        bool Read(string name, ref string[] value);

        bool Read<T>(string name, ref List<T> value) where T : ISerial, new();

        bool Read<K, V>(string name, ref Dictionary<K, V> value);
    }
}