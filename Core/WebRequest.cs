using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
    ///
    /// The supported content typess
    ///
    enum CType
    {
        Form,
        MultipartForm,
        Json,
        Xml,
        Stream,
    }

    public class WebRequest : IDataInput
    {
        private readonly HttpRequest _impl;

        private CType _ctype;

        private IFormCollection _form;

        private JsonCodec _json;

        internal WebRequest(HttpRequest impl)
        {
            _impl = impl;

            _form = null;
            _json = null;
            string ctype = impl.ContentType;
            if ("application/x-www-form-urlencoded".Equals(ctype))
            {
                _ctype = CType.Form;
                _form = impl.Form;
            }
            else if ("application/json".Equals(ctype))
            {
                _json = new JsonCodec(12);
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