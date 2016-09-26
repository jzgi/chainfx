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

        bool Array(Action a);

        bool Object(Action a);

        //
        // value reading

        bool Read(ref bool value);

        bool Read(ref short value);

        bool Read(ref int value);

        bool Read(ref long value);

        bool Read(ref decimal value);

        bool Read(ref DateTime value);

        bool Read(ref char[] value);

        bool Read(ref string value);

        bool Read<T>(ref T value) where T : ISerial, new();

        bool Read(ref byte[] value);

        bool Read<T>(ref List<T> value);

        bool Read<T>(ref Dictionary<string, T> value);

        //
        // property reading

        bool Read(string name, ref bool value);

        bool Read(string name, ref short value);

        bool Read(string name, ref int value);

        bool Read(string name, ref long value);

        bool Read(string name, ref decimal value);

        bool Read(string name, ref DateTime value);

        bool Read(string name, ref char[] value);

        bool Read(string name, ref string value);

        bool Read<T>(string name, ref T value) where T : ISerial, new();

        bool Read(string name, ref byte[] value);

        bool Read<T>(string name, ref List<T> value);

        bool Read<T>(string name, ref Dictionary<string, T> value);
    }
}