using Greatbone.Core;
using Microsoft.Extensions.Primitives;

namespace Greatbone.Sample
{
    public abstract class SampleService : WebService
    {
        public SampleService(WebConfig cfg) : base(cfg)
        {
        }

        protected override bool Authenticate(WebContext wc)
        {
            StringValues h;
            if (wc.Request.Headers.TryGetValue("Authorization", out h))
            {
                string v = (string)h;
                v.StartsWith("Bearer "); // Bearer scheme
                JTextParse parse = new JTextParse(v.Substring(7));
                JObj jo = (JObj)parse.Parse();
                IToken tok = new Token();
                tok.Load(jo);
                wc.Token = tok;

                return true;
            }
            return false;
        }
    }
}