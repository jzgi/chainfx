using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonText : ISink<JsonText>
    {
        // for parsing json text
        string text;

        // for building json text
        char[] buffer;

        int count;

        int pos;

        // parsing context for levels

        int[] starts = new int[8];

        int level;

        public JsonText(int capacity)
        {
            buffer = new char[capacity];
        }

        public JsonText(string json)
        {
            text = json;
        }

        //
        // READ
        //

        public void Arr(Action a)
        {

        }

        public void Obj(Action a)
        {


        }

        public JsonText Put(string name, bool value)
        {
            throw new NotImplementedException();
        }

        public JsonText Put(string name, short value)
        {
            throw new NotImplementedException();
        }

        public JsonText Put(string name, int value)
        {
            throw new NotImplementedException();
        }

        public JsonText Put(string name, long value)
        {
            throw new NotImplementedException();
        }

        public JsonText Put(string name, decimal value)
        {
            throw new NotImplementedException();
        }

        public JsonText Put(string name, DateTime value)
        {
            throw new NotImplementedException();
        }

        public JsonText Put(string name, string value)
        {
            throw new NotImplementedException();
        }

        public JsonText Put<T>(string name, T value, int x) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public JsonText Put<T>(string name, List<T> value, int x) where T : IPersist
        {
            throw new NotImplementedException();
        }

        public JsonText Put(string name, byte[] value)
        {
            throw new NotImplementedException();
        }

        public JsonText Put(string name, Obj value)
        {
            throw new NotImplementedException();
        }

        public JsonText Put(string name, Arr value)
        {
            throw new NotImplementedException();
        }

        //
        // WRITE
        //
    }
}