using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
    public class WebRequest : IDataInput
    {
        private readonly HttpRequest _impl;

        private IFormCollection _form;

        private JsonBin _json;

        internal WebRequest(HttpRequest impl)
        {
            _impl = impl;

            _form = null;
            _json = null;
            string ctype = impl.ContentType;
            if ("application/x-www-form-urlencoded".Equals(ctype))
            {
                _form = impl.Form;
            }
            else if ("application/json".Equals(ctype))
            {
                _json = new JsonBin(12);
            }
        }

        public bool GotStart()
        {
            throw new System.NotImplementedException();
        }

        public bool GotEnd()
        {
            throw new System.NotImplementedException();
        }

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

        public bool Got<T>(string name, out List<T> value) where T : IData
        {
            throw new System.NotImplementedException();
        }
    }
}