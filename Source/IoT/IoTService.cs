using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using SkyCloud.Web;

namespace SkyCloud.IoT
{
    /// <summary>
    /// A web service that realizes API for both inter-node communication and 
    /// </summary>
    public class IoTService : WebService
    {
        readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();


        // edge devices
        private ConcurrentDictionary<string, Device> devs;

        // the thread schedules and drives periodic jobs, such as event polling 
        Thread scheduler;


        protected internal override void OnCreate()
        {
            // ensure DDL

            // load and setup peers

            // create and start the scheduler thead
        }


        [Get("Get access token for a login", query: "?id=<-login-id->&password=")]
        [Reply(200, "Success", body: @"{
            name : string, // login name
            id : string, // login id
            token : // access token
        }")]
        [Reply(404, "login not found or invalid password")]
        public void token(WebContext wc)
        {
            string id = wc.Query[nameof(id)];
            string password = wc.Query[nameof(password)];

            using var dc = NewDbContext();

            // retrieve from idents
        }

        public async Task accept(WebContext wc)
        {
            // device token
            string token = null;

            // identity id of client
            string uid = null;

            var dev = devs[uid];
            dev.Socket = await wc.AcceptWebSocketAsync();
        }

        public void querya(WebContext wc)
        {
        }

        public void put(WebContext wc)
        {
        }
    }
}