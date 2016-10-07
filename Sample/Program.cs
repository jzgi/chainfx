using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    ///
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry point of the application.
        /// </summary>
        public static void Main(string[] args)
        {
            DbConfig pg = new DbConfig
            {
                Host = "60.205.104.239",
                Port = 5432,
                Username = "postgres",
                Password = "Zou###1989",
                MQ = false
            };

            string[] addrs =             {
                "127.0.0.1:7070",
                "127.0.0.1:7071",
                "127.0.0.1:7072",
                "127.0.0.1:7073",
                "127.0.0.1:7074",
            };

            var www = new WwwService(new WebConfig
            {
                key = "www",
                @public = "127.0.0.1:8080",
                @private = "127.0.0.1:7070",
                net = addrs,
                db = pg
            }
#if !DEBUG
            .LoadFile("www.json")
#endif
            );

            var biz = new BizService(new WebConfig
            {
                key = "biz",
                @public = "127.0.0.1:8081",
                @private = "127.0.0.1:7071",
                net = addrs,
                db = pg
            }
#if !DEBUG
            .LoadFile("biz.json")
#endif
            );

            var cont = new ContService(new WebConfig
            {
                key = "cont",
                @public = "127.0.0.1:8082",
                @private = "127.0.0.1:7072",
                net = addrs,
                db = pg
            }
#if !DEBUG
            .LoadFile("cont.json")
#endif
            );

            var dir = new DirService(new WebConfig
            {
                key = "dir",
                @public = "127.0.0.1:8083",
                @private = "127.0.0.1:7073",
                net = addrs,
                db = pg
            }
#if !DEBUG
            .LoadFile("dir.json")
#endif
            );

            var chat = new ChatService(new WebConfig
            {
                key = "chat",
                @public = "127.0.0.1:8084",
                @private = "127.0.0.1:7074",
                net = addrs,
                db = pg
            }
#if !DEBUG
            .LoadFile("chat.json")
#endif
            );

            WebService.Run(www, biz, cont, dir, chat);
        }
    }
}