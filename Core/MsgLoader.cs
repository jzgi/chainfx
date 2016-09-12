using System;
using System.Collections.Generic;
using System.Text;

namespace Greatbone.Core
{
    public class MsgLoader : IMember
    {
        readonly WebService service;

        string key;

        Queue<MsgMessage> cache;

        string sql;

        public string Key => key;


        internal MsgLoader(WebService svc)
        {
            service = svc;

            StringBuilder sb = new StringBuilder("SELECT * FROM mqueue WHERE id > @lastid AND ");
            for (int i = 0; i < svc.Subscribes.Count; i++)
            {
                MsgSubscribe sub = svc.Subscribes[i];

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
                using (var dc = service.NewSqlContext())
                {
                    dc.Query("SELECT * FROM mqueue WHERE id > @lastid AND ", null);
                }
            }
        }

    }

}