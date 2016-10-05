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
                Key = "www",
                Public = "127.0.0.1:8080",
                Private = "127.0.0.1:7070",
                Net = addrs,
                Db = pg
            }
#if !DEBUG
            .LoadFile("www.json")
#endif
            );

            var biz = new BizService(new WebConfig
            {
                Key = "biz",
                Public = "127.0.0.1:8081",
                Private = "127.0.0.1:7071",
                Net = addrs,
                Db = pg
            }
#if !DEBUG
            .LoadFile("biz.json")
#endif
            );

            var cont = new ContService(new WebConfig
            {
                Key = "cont",
                Public = "127.0.0.1:8082",
                Private = "127.0.0.1:7072",
                Net = addrs,
                Db = pg
            }
#if !DEBUG
            .LoadFile("cont.json")
#endif
            );

            var dir = new DirService(new WebConfig
            {
                Key = "dir",
                Public = "127.0.0.1:8083",
                Private = "127.0.0.1:7073",
                Net = addrs,
                Db = pg
            }
#if !DEBUG
            .LoadFile("dir.json")
#endif
            );

            var chat = new ChatService(new WebConfig
            {
                Key = "chat",
                Public = "127.0.0.1:8084",
                Private = "127.0.0.1:7074",
                Net = addrs,
                Db = pg
            }
#if !DEBUG
            .LoadFile("chat.json")
#endif
            );

            WebService.Run(www, biz, cont, dir, chat);
        }
    }
}