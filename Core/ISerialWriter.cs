using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary> <summary>
    /// A serial content writer.
    /// </summary>
    public interface ISerialWriter
    {

        //
        // knot writing

        bool WriteArray(Action a);

        bool WriteObject(Action a);

        //
        // value writing

        bool Write(bool value);

        bool Write(short value);

        bool Write(int value);

        bool Write(long value);

        bool Write(decimal value);

        bool Write(DateTime value);

        bool Write(char[] value);

        bool Write(string value);

        bool Write<T>(T value) where T : ISerial;

        bool Write<T>(T[] value);

        bool Write<T>(List<T> value);

        bool Write<T>(Dictionary<string, T> value);

        //
        // property writing

        bool Write(string name, bool value);

        bool Write(string name, short value);

        bool Write(string name, int value);

        bool Write(string name, long value);

        bool Write(string name, decimal value);

        bool Write(string name, DateTime value);

        bool Write(string name, char[] value);

        bool Write(string name, string value);

        bool Write<T>(string name, T value) where T : ISerial;

        bool Write<T>(string name, T[] value);

        bool Write<T>(string name, List<T> value);

        bool Write<T>(string name, Dictionary<string, T> value);
    }
}