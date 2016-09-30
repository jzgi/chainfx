using System.Collections.Generic;
using System.Text;

namespace Greatbone.Core
{
    public class MsgLoader : IKeyed
    {

        WebService service;

        string key;

        Queue<MsgMessage> cache;

        string sql;

        public string Key => key;

        Roll<MsgAction> mactions ;

        internal MsgLoader( WebService service)
        {
            this.service = service;

            StringBuilder sb = new StringBuilder("SELECT * FROM mqueue WHERE id > @lastid AND ");
            for (int i = 0; i < mactions.Count; i++)
            {
                MsgAction sub = mactions[i];

                sb.Append("topic = '").Append(sub.Topic).Append("'");
            }
            sql = sb.ToString();

        }

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