using Greatbone.Core;

namespace Greatbone.Sample
{

    public class Program
    {
        /// <summary>
        /// The entry point of the application.
        /// </summary>
        public static void Main(string[] args)
        {

            DbConfig pg = new DbConfig
            {
                host = "60.205.104.239",
                port = 5432,
                username = "postgres",
                password = "Zou###1989",
                msg = false
            };

            string[] addrs = {
                "127.0.0.1:7070",
                "127.0.0.1:7071",
                "127.0.0.1:7072",
                "127.0.0.1:7073",
                "127.0.0.1:7074",
            };

            var www = new WwwService(new WebConfig
            {
                key = "www",
                @extern = "192.168.1.38:8080",
                intern = "127.0.0.1:7070",
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
                @extern = "127.0.0.1:8081",
                intern = "127.0.0.1:7071",
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
                @extern = "127.0.0.1:8082",
                intern = "127.0.0.1:7072",
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
                @extern = "127.0.0.1:8083",
                intern = "127.0.0.1:7073",
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
                @extern = "127.0.0.1:8084",
                intern = "127.0.0.1:7074",
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