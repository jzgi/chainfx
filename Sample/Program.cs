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
            // JsonContentTest.Test();

            DbConfig pg = new DbConfig
            {
                Host = "60.205.104.239",
                Port = 5432,
                Username = "postgres",
                Password = "Zou###1989",
                MQ = false
            };

            string[] addrs = {
                "127.0.0.8081",
                "127.0.0.8082",
                "127.0.0.8083",
                "127.0.0.8084",
                "127.0.0.8085",
            };

            var www = new WwwService(new WebServiceConfig
            {
                Key = "www",
                Public = "127.0.0.1:8080",
                Private = "127.0.0.1:7770",
                Cluster = addrs,
                Db = pg,
                Debug = true
            }.Load("www.json")
            );

            var fame = new FameService(new WebServiceConfig
            {
                Key = "fame",
                Public = "127.0.0.1:8081",
                Private = "127.0.0.1:7771",
                Cluster = addrs,
                Db = pg,
                Debug = true
            }.Load("fame.json")
            );

            var brand = new BrandService(new WebServiceConfig
            {
                Key = "biz",
                Public = "127.0.0.1:8082",
                Private = "127.0.0.1:7772",
                Cluster = addrs,
                Db = pg,
                Debug = true
            }.Load("biz.json")
            );

            var post = new PostService(new WebServiceConfig
            {
                Key = "post",
                Public = "127.0.0.1:8083",
                Private = "127.0.0.1:7773",
                Cluster = addrs,
                Db = pg,
                Debug = true
            }.Load("post.json")
            );

            var notice = new NoticeService(new WebServiceConfig
            {
                Key = "notice",
                Public = "127.0.0.1:8084",
                Private = "127.0.0.1:7774",
                Cluster = addrs,
                Db = pg,
                Debug = true
            }.Load("notice.json")
            );

            var user = new UserService(new WebServiceConfig
            {
                Key = "user",
                Public = "127.0.0.1:8085",
                Private = "127.0.0.1:7775",
                Cluster = addrs,
                Db = pg,
                Debug = true
            }.Load("user.json")
            );

            var chat = new ChatService(new WebServiceConfig
            {
                Key = "chat",
                Public = "127.0.0.1:8086",
                Private = "127.0.0.1:7776",
                Cluster = addrs,
                Db = pg,
                Debug = true
            }.Load("chat.json")
            );

            WebService.Run(www, fame, brand, post, notice, user, chat);
        }
    }
}