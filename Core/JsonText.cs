using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonText : IOutput
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

        public void Put(string name, bool value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, short value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, int value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, long value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, decimal value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, DateTime value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, char[] value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, string value)
        {
            throw new NotImplementedException();
        }

     
        public void Put(string name, byte[] value)
        {
            throw new NotImplementedException();
        }

        public void Put<T>(string name, List<T> value)
        {
            throw new NotImplementedException();
        }

        public void Put<T>(string name, Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public void Put<T>(string name, T value)
        {
            throw new NotImplementedException();
        }

        //
        // WRITE
        //
    }
}