using System.Collections.Generic;
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
            DbConfig pg = new DbConfig
            {
                host = "106.14.45.109",
                port = 5432,
                username = "postgres",
                password = "721004",
                eq = false
            };

            WebAuth<Token> auth = new WebAuth<Token>(0x4a78be76, 0x1f0335e2);

            List<WebService> svcs = new List<WebService>(4);

            WebConfig cfg;

            cfg = new WebConfig("shop")
            {
                addresses = "http://127.0.0.1:8080",
                db = pg
            };
#if !DEBUG
            cfg.TryLoad();
#endif
            if (cfg.File != false) svcs.Add(new ShopService(cfg) { Auth = auth });

            cfg = new WebConfig("comm")
            {
                addresses = "http://127.0.0.1:8081",
                db = pg
            };
#if !DEBUG
            cfg.TryLoad();
#endif

            if (cfg.File != false) svcs.Add(new CommService(cfg) { Auth = auth });

            WebService.Run(svcs);
        }
    }
}