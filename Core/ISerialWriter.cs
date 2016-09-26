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

        bool Array(Action a);

        bool Object(Action a);

        //
        // value writing

        void Write(bool value);

        void Write(short value);

        void Write(int value);

        void Write(long value);

        void Write(decimal value);

        void Write(DateTime value);

        void Write(char[] value);

        void Write(string value);

        void Write(byte[] value);

        void Write<T>(T value) where T : ISerial;

        void Write<T>(List<T> value);

        void Write<T>(Dictionary<string, T> value);

        //
        // property writing

        void Write(string name, bool value);

        void Write(string name, short value);

        void Write(string name, int value);

        void Write(string name, long value);

        void Write(string name, decimal value);

        void Write(string name, DateTime value);

        void Write(string name, char[] value);

        void Write(string name, string value);

        void Write<T>(string name, T value) where T : ISerial;

        void Write(string name, byte[] value);

        void Write<T>(string name, List<T> value);

        void Write<T>(string name, Dictionary<string, T> value);
    }
}