using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonString : ISource
    {
        private string str;

        public bool Got(string name, out int value)
        {
            throw new System.NotImplementedException();
        }

        public bool Got(string name, out decimal value)
        {
            throw new System.NotImplementedException();
        }

        public bool Got(string name, out string value)
        {
            throw new System.NotImplementedException();
        }

        public bool Got<T>(string name, out List<T> value) where T : IPersist
        {
            throw new System.NotImplementedException();
        }
    }
}