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

            WebAuth<Shop, Shop> auth = new WebAuth<Shop, Shop>(0x4a78be76, 0x1f0335e2);

            List<WebService> svclst = new List<WebService>(4);

            WebConfig cfg;

            cfg = new WebConfig("shop")
            {
                pub = "http://127.0.0.1:8080",
                intern = "http://127.0.0.1:7070",
                db = pg
            };
#if !DEBUG
            cfg.TryImport();
#endif
            if (cfg.Import != false) svclst.Add(new ShopService(cfg) { Auth = auth });

            cfg = new WebConfig("chat")
            {
                pub = "http://127.0.0.1:8081",
                intern = "http://127.0.0.1:7071",
                db = pg
            };
#if !DEBUG
            cfg.TryImport();
#endif

            if (cfg.Import != false) svclst.Add(new ChatService(cfg) { Auth = auth });

            WebService.Run(svclst);
        }
    }
}