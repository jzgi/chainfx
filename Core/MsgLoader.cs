using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class MsgLoader : IMember
    {
        WebService service;

        string key;

        Queue<Message> cache;

        public string Key => key;


        public void Get()
        {
            Message msg;
            if (cache.Count > 0)
            {
                msg = cache.Dequeue();
            }
            else
            {
                using (var dc = service.NewSqlContext())
                {
                    dc.Query("SELECT * FROM mqueue WHERE id > @lastid AND ", null);
                }
            }
        }

        internal struct Message : IContent
        {
            byte[] body;

            string topic;

            public byte[] Buffer
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int Count
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public long ETag
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public DateTime LastModified
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string Type => "msg/bin";

        }

    }

}