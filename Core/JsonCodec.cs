using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Greatbone.Core
{
    ///
    /// A test-based JSON builder & parser.
    ///
    public class JsonCodec : IDataInput, IDataOutput
    {
        // source string to parse
        private readonly string _str;

        // current position
        private int _pos;

        // buffer for string build
        private char[] _buffer;


        public JsonCodec() : this(2048)
        {
        }

        public JsonCodec(int capacity)
        {
            _buffer = new char[capacity];
        }

        public JsonCodec(string str)
        {
            _str = str;
        }

        public bool IsCoder => _str != null;

        public bool IsDecoder => _str == null;

        public bool GotStart()
        {
            while (_pos < _str.Length)
            {
                if (_str[_pos++] == '{')
                {
                    return true;
                }
            }
            return false;
        }

        public bool GotEnd()
        {
            while (_pos < _str.Length)
            {
                if (_str[_pos++] == '}')
                {
                    return true;
                }
            }
            return false;
        }

        public bool Got(string name, out int value)
        {
            while (_pos < _str.Length)
            {
                if (_str[_pos++] == '}')
                {
                    value = 1;
                    return true;
                }
            }
            value = 0;
            return false;
        }


        public bool Got(string name, out decimal value)
        {
            throw new System.NotImplementedException();
        }

        public bool Got(string name, out string value)
        {
            throw new System.NotImplementedException();
        }

        public bool Got<T>(string name, out List<T> value) where T : IData
        {
            throw new System.NotImplementedException();
        }


        public void PutStart()
        {
            throw new System.NotImplementedException();
        }

        public void PutEnd()
        {
            throw new System.NotImplementedException();
        }

        public void Put(string name, int value)
        {
            throw new System.NotImplementedException();
        }

        public void Put(string name, decimal value)
        {
            throw new System.NotImplementedException();
        }

        public void Put(string name, string value)
        {
            throw new System.NotImplementedException();
        }

        public void Put<T>(string name, List<T> value) where T : IData
        {
            throw new System.NotImplementedException();
        }
    }
}