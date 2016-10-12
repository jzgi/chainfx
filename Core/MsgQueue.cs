using System.Collections.Generic;

namespace Greatbone.Core
{
    public class MsgQueue : IKeyed
    {
        readonly WebService service;

        readonly string addr;

        // queued for sending out
        Queue<MsgMessage> cache;

        // SQL statement for selecting local messages
        string sql;

        Roll<MsgHook> mactions;

        internal MsgQueue(WebService service, string addr)
        {
            this.service = service;
            this.addr = addr;

            // StringBuilder sb = new StringBuilder("SELECT * FROM mqueue WHERE id > @lastid AND ");
            // for (int i = 0; i < mactions.Count; i++)
            // {
            //     MsgAction sub = mactions[i];

            //     sb.Append("topic = '").Append(sub.Key).Append("'");
            // }
            // sql = sb.ToString();

        }

        public string Key => addr;

        public void Get()
        {
            MsgMessage item;
            if (cache.Count > 0)
            {
                item = cache.Dequeue();
            }
            else
            {
                using (var dc = service.NewDbContext())
                {
                    dc.Query("SELECT * FROM mqueue WHERE id > @lastid AND ", null);
                }
            }
        }

    }

}