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
            AuthConfig auth = new AuthConfig
            {
                mask = 0x4a78be76,
                repos = 0x1f0335e2,
                maxage = 3600
            };

            DbConfig pg = new DbConfig
            {
                host = "106.14.45.109",
                port = 5432,
                username = "postgres",
                password = "721004",
                queue = false
            };

            List<WebService> svclst = new List<WebService>(4);

            WebServiceContext sc;

            sc = new WebServiceContext("op")
            {
                addresses = "http://localhost:8080",
                auth = auth,
                db = pg
            };
#if !DEBUG
            sc.TryLoad();
#endif
            if (sc.LoadedOk != false) svclst.Add(new OpService(sc));

            string tree = svclst[0].Describe();
            Debug.WriteLine(tree);

            sc = new WebServiceContext("comm")
            {
                addresses = "http://localhost:8081",
                auth = auth,
                db = pg
            };
#if !DEBUG
            sc.TryLoad();
#endif

            if (sc.LoadedOk != false) svclst.Add(new CommService(sc));

            WebService.Run(svclst);
        }
    }
}