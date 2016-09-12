using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    ///
    /// </summary>
    public class Program
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            JsonContentTest.Test();

            DbConfig pg = new DbConfig
            {
                host = "60.205.104.239",
                port = 5432,
                username = "postgres",
                password = "Zou###1989"
            };

            var www = new WwwService(new WebServiceConfig
            {
                key = "www",
                outer = "127.0.0.1:8080",
                inner = "127.0.0.1:7770",
                cluster = new[] {
                        "localhost:7777"
                    },
                db = pg,
                debug = true
            }.Load("www.json")
            );

            var fame = new FameService(new WebServiceConfig
            {
                key = "fame",
                outer = "127.0.0.1:8081",
                inner = "127.0.0.1:7771",
                cluster = new[]
                    {
                        "localhost:7777"
                    },
                db = pg,
                debug = true
            }.Load("fame.json")
            );

            var brand = new BrandService(new WebServiceConfig
            {
                key = "biz",
                outer = "127.0.0.1:8082",
                inner = "127.0.0.1:7772",
                cluster = new[]
                    {
                        "127.0.0.1:7777"
                    },
                db = pg,
                debug = true
            }.Load("biz.json")
            );

            var post = new PostService(new WebServiceConfig
            {
                key = "post",
                outer = "127.0.0.1:8083",
                inner = "127.0.0.1:7773",
                cluster = new[]
                    {
                        "localhost:7777"
                    },
                db = pg,
                debug = true
            }.Load("post.json")
            );

            var notice = new NoticeService(new WebServiceConfig
            {
                key = "notice",
                outer = "127.0.0.1:8084",
                inner = "127.0.0.1:7774",
                cluster = new[]
                    {
                        "localhost:7783"
                    },
                db = pg,
                debug = true
            }.Load("notice.json")
            );

            var user = new UserService(new WebServiceConfig
            {
                key = "user",
                outer = "127.0.0.1:8085",
                inner = "127.0.0.1:7775",
                cluster = new[]
                    {
                        "localhost:7783"
                    },
                db = pg,
                debug = true
            }.Load("user.json")
            );

            var chat = new ChatService(new WebServiceConfig
            {
                key = "chat",
                outer = "127.0.0.1:8086",
                inner = "127.0.0.1:7776",
                cluster = new[]
                    {
                        "localhost:7783"
                    },
                db = pg,
                debug = true
            }.Load("chat.json")
            );

            WebService.Run(www, fame, brand, post, notice, user, chat);
        }
    }
}