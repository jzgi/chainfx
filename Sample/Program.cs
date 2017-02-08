using System.Collections.Generic;
using System.Diagnostics;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Program
    {
        ///
        /// The program's entry point.
        ///
        public static void Main(string[] args)
        {
            // string xml = "<xml><log file=\"abc.log\" level=\"&lt;3&gt;\"><reserve/></log><display>CRT-&amp;</display></xml>";
            // byte[] bytes = Encoding.UTF8.GetBytes(xml);
            // XElem e = new XmlParse(bytes, 0, bytes.Length).Parse();

            DbConfig pg = new DbConfig
            {
                host = "106.14.45.109",
                port = 5432,
                username = "postgres",
                password = "721004",
                queue = false
            };

            WebAuthent<Token> auth = new WebAuthent<Token>(0x4a78be76, 0x1f0335e2);

            List<WebService> svclst = new List<WebService>(4);

            WebServiceContext sc;

            sc = new WebServiceContext("op")
            {
                addresses = "http://localhost:8080",
                db = pg
            };
#if !DEBUG
            sc.TryLoad();
#endif
            if (sc.LoadedOk != false) svclst.Add(new OpService(sc) { Authent = auth });

            string tree = svclst[0].Describe();
            Debug.WriteLine(tree);

            sc = new WebServiceContext("comm")
            {
                addresses = "http://localhost:8081",
                db = pg
            };
#if !DEBUG
            sc.TryLoad();
#endif

            if (sc.LoadedOk != false) svclst.Add(new CommService(sc) { Authent = auth });

            WebService.Run(svclst);
        }
    }
}