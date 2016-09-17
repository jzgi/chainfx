using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonText : ISerialReader, ISerialWriter
    {

       private string text;

        // for output
        private char[] buffer;

        private int pos;

        // parsing context for levels

        JsonStruct[] levels = new JsonStruct[8];

        int level;

        public JsonText(int capacity)
        {
        }

        public JsonText(string str)
        {
            this.text = str;
        }

        //
        // READ
        //

        public bool ReadArray(Action a)
        {
            throw new NotImplementedException();
        }

        public bool ReadObject(Action a)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref bool value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref short value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref int value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref long value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref decimal value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref DateTime value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read(ref string value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(ref T value) where T : ISerial, new()
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(ref T[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(ref List<T> value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(ref Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref bool value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref short value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref int value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref long value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref decimal value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref DateTime value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref char[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read(string name, ref string value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(string name, ref T value) where T : ISerial, new()
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(string name, ref T[] value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(string name, ref List<T> value)
        {
            throw new NotImplementedException();
        }

        public bool Read<T>(string name, ref Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public void WriteArray(Action a)
        {
            throw new NotImplementedException();
        }

        public void WriteObject(Action a)
        {
            throw new NotImplementedException();
        }

        public void Write(bool value)
        {
            throw new NotImplementedException();
        }

        public void Write(short value)
        {
            throw new NotImplementedException();
        }

        public void Write(int value)
        {
            throw new NotImplementedException();
        }

        public void Write(long value)
        {
            throw new NotImplementedException();
        }

        public void Write(decimal value)
        {
            throw new NotImplementedException();
        }

        public void Write(DateTime value)
        {
            throw new NotImplementedException();
        }

        public void Write(char[] value)
        {
            throw new NotImplementedException();
        }

        public void Write(string value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(T value) where T : ISerial
        {
            throw new NotImplementedException();
        }

        public void Write<T>(T[] value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(List<T> value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, bool value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, short value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, int value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, long value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, decimal value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, DateTime value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, char[] value)
        {
            throw new NotImplementedException();
        }

        public void Write(string name, string value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(string name, T value) where T : ISerial
        {
            throw new NotImplementedException();
        }

        public void Write<T>(string name, T[] value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(string name, List<T> value)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(string name, Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        //
        // WRITE
        //

    }
}