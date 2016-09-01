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

            var www = new WwwService(new WebServiceContext
                {
                    key = "WWW",
                    @public = "127.0.0.1:8080",
                    @internal = "127.0.0.1:7770",
                    foreign = new[]
                    {
                        "localhost:7777"
                    },
                    datasrc = dat,
                    debug = true
                }.Load("WWW.json")
            );

            var fame = new FameService(new WebServiceContext
                {
                    key = "BIZ",
                    @public = "127.0.0.1:8081",
                    @internal = "127.0.0.1:7771",
                    foreign = new[]
                    {
                        "localhost:7777"
                    },
                    datasrc = dat,
                    debug = true
                }.Load("BIZ.json")
            );

            var brand = new BrandService(new WebServiceContext
                {
                    key = "BIZ",
                    @public = "127.0.0.1:8082",
                    @internal = "127.0.0.1:7772",
                    foreign = new[]
                    {
                        "127.0.0.1:7777"
                    },
                    datasrc = dat,
                    debug = true
                }.Load("BIZ.json")
            );

            var post = new PostService(new WebServiceContext
                {
                    key = "POST",
                    @public = "127.0.0.1:8083",
                    @internal = "127.0.0.1:7773",
                    foreign = new[]
                    {
                        "localhost:7777"
                    },
                    datasrc = dat,
                    debug = true
                }.Load("POST.json")
            );

            var notice = new NoticeService(
                new WebServiceContext
                {
                    key = "NOTICE",
                    @public = "127.0.0.1:8084",
                    @internal = "127.0.0.1:7774",
                    foreign = new[]
                    {
                        "localhost:7783"
                    },
                    datasrc = dat,
                    debug = true
                }.Load("NOTICE.json")
            );

            var user = new UserService(new WebServiceContext
                {
                    key = "USER",
                    @public = "127.0.0.1:8085",
                    @internal = "127.0.0.1:7775",
                    foreign = new[]
                    {
                        "localhost:7783"
                    },
                    datasrc = dat,
                    debug = true
                }.Load("USER.json")
            );

            var chat = new ChatService(new WebServiceContext
                {
                    key = "CHAT",
                    @public = "127.0.0.1:8086",
                    @internal = "127.0.0.1:7776",
                    foreign = new[]
                    {
                        "localhost:7783"
                    },
                    datasrc = dat,
                    debug = true
                }.Load("MSG.json")
            );

            WebService.Run(www, fame, brand, post, notice, user, chat);
        }
    }
}