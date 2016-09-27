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

            List<string> addrs = new List<string>
            {
                "127.0.0.7070",
                "127.0.0.7071",
                "127.0.0.7072",
                "127.0.0.7073",
                "127.0.0.7074",
            };

            var www = new WwwService(new WebServiceConfig
            {
                Key = "www",
                Public = "127.0.0.1:8080",
                Private = "127.0.0.1:7070",
                Net = addrs,
                Db = pg,
                Debug = true
            }.Load("www.json"));

            var biz = new BizService(new WebServiceConfig
            {
                Key = "biz",
                Public = "127.0.0.1:8081",
                Private = "127.0.0.1:7071",
                Net = addrs,
                Db = pg,
                Debug = true
            }.Load("biz.json"));

            var cont = new ContService(new WebServiceConfig
            {
                Key = "cont",
                Public = "127.0.0.1:8082",
                Private = "127.0.0.1:7072",
                Net = addrs,
                Db = pg,
                Debug = true
            }.Load("cont.json"));

            var dir = new DirService(new WebServiceConfig
            {
                Key = "dir",
                Public = "127.0.0.1:8083",
                Private = "127.0.0.1:7073",
                Net = addrs,
                Db = pg,
                Debug = true
            }.Load("dir.json"));

            var chat = new ChatService(new WebServiceConfig
            {
                Key = "chat",
                Public = "127.0.0.1:8084",
                Private = "127.0.0.1:7074",
                Net = addrs,
                Db = pg,
                Debug = true
            }.Load("chat.json"));

            WebService.Run(www, biz, cont, dir, chat);
        }
    }
}