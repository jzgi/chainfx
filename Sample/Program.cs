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
            DataSrcBuilder dat = new DataSrcBuilder
            {
                host = "localhost",
                port = 5432,
                username = "postgres",
                password = "Zou###1989"
            };

            var www = new WwwService(new WebServiceBuilder
            {
                key = "www",
                outer = "127.0.0.1:8080",
                inner = "127.0.0.1:7770",
                foreign = new[] {
                        "localhost:7777"
                    },
                datasrc = dat,
                debug = true
            }.Load("www.json")
            );

            var fame = new FameService(new WebServiceBuilder
            {
                key = "fame",
                outer = "127.0.0.1:8081",
                inner = "127.0.0.1:7771",
                foreign = new[]
                    {
                        "localhost:7777"
                    },
                datasrc = dat,
                debug = true
            }.Load("fame.json")
            );

            var brand = new BrandService(new WebServiceBuilder
            {
                key = "biz",
                outer = "127.0.0.1:8082",
                inner = "127.0.0.1:7772",
                foreign = new[]
                    {
                        "127.0.0.1:7777"
                    },
                datasrc = dat,
                debug = true
            }.Load("biz.json")
            );

            var post = new PostService(new WebServiceBuilder
            {
                key = "post",
                outer = "127.0.0.1:8083",
                inner = "127.0.0.1:7773",
                foreign = new[]
                    {
                        "localhost:7777"
                    },
                datasrc = dat,
                debug = true
            }.Load("post.json")
            );

            var notice = new NoticeService(new WebServiceBuilder
            {
                key = "notice",
                outer = "127.0.0.1:8084",
                inner = "127.0.0.1:7774",
                foreign = new[]
                    {
                        "localhost:7783"
                    },
                datasrc = dat,
                debug = true
            }.Load("notice.json")
            );

            var user = new UserService(new WebServiceBuilder
            {
                key = "user",
                outer = "127.0.0.1:8085",
                inner = "127.0.0.1:7775",
                foreign = new[]
                    {
                        "localhost:7783"
                    },
                datasrc = dat,
                debug = true
            }.Load("user.json")
            );

            var chat = new ChatService(new WebServiceBuilder
            {
                key = "chat",
                outer = "127.0.0.1:8086",
                inner = "127.0.0.1:7776",
                foreign = new[]
                    {
                        "localhost:7783"
                    },
                datasrc = dat,
                debug = true
            }.Load("chat.json")
            );

            WebService.Run(www, fame, brand, post, notice, user, chat);
        }
    }
}