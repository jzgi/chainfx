using System;

namespace Greatbone.Core
{
    /// 
    /// A event queue pertaining to a certain event client.
    /// 
    public class EventQueue : IRollable
    {
        readonly string name;

        readonly Event[] elements;

        int head;

        int tail;

        int count;

        internal long lastid;

        internal EventQueue(string name, int capacity)
        {
            this.name = name;
            elements = new Event[capacity];
        }

        public string Name => name;

        public void Poll(ActionContext ac)
        {
            return;
        }

        public void Clear()
        {
            head = 0;
            tail = 0;
            count = 0;
        }

        //
        // static
        //

        internal static void GlobalInit(Service service, Roll<EventQueue> queues)
        {
            using (var dc = service.NewDbContext())
            {
                // create database objects

                dc.Execute(@"CREATE TABLE IF NOT EXISTS EVTU (
                                moniker varchar(20),
                                lastid int8
                                CONSTRAINT evtu_pkey PRIMARY KEY (moniker)
                            ) WITH (OIDS=FALSE)"
                );

                dc.Execute(@"CREATE SEQUENCE IF NOT EXISTS evtq_id_seq 
                                INCREMENT 1 
                                MINVALUE 1 MAXVALUE 9223372036854775807 
                                START 1 CACHE 100 NO CYCLE
                                OWNED BY EVTQ.id"
                );

                dc.Execute(@"CREATE TABLE IF NOT EXISTS EVTQ (
                                id int8 DEFAULT nextval('evtq_id_seq'::regclass) NOT NULL,
                                name varchar(40),
                                shard varchar(20),
                                arg varchar(40),
                                type varchar(40),
                                body bytea,
                                time timestamp
                                CONSTRAINT evtq_pkey PRIMARY KEY (id)
                            ) WITH (OIDS=FALSE)"
                );

                // init records for each moniker 

                for (int i = 0; i < queues.Count; i++)
                {
                    EventQueue que = queues[i];
                    if (dc.Query1("SELECT lastid FROM EVTU WHERE moniker = @1", p => p.Set(que.Name)))
                    {
                        que.lastid = dc.GetLong();
                    }
                    else
                    {
                        dc.Execute("INSERT INTO EVTU (moniker, lastid) VALUES (@1, @2)", p => p.Set(que.Name).Set(que.lastid));
                    }
                }
            }
        }

        static void Post(DbContext dc, string name, string shard, string arg, IContent content)
        {
            if (content == null)
            {
                dc.Execute("INSERT INTO EVTQ (name, shard, arg) VALUES (@1, @2, @3)", p =>
                {
                    p.Set(name).Set(shard).Set(arg);
                });
            }
            else
            {
                var body = new ArraySegment<byte>(content.ByteBuffer, 0, content.Size);
                dc.Execute("INSERT INTO EVTQ (name, shard, arg, type, body) VALUES (@1, @2, @3, @4, @5)", p =>
                {
                    p.Set(name).Set(shard).Set(arg).Set(content.Type).Set(body);
                });
            }
        }

        internal static void PeekEq(ActionContext ac, string[] names, string shard, long? lastid)
        {
            using (var dc = ac.NewDbContext())
            {
                DbSql sql = dc.Sql("SELECT * FROM evtq WHERE id > @1 AND name IN [");
                for (int i = 0; i < names.Length; i++)
                {
                    if (i > 0) sql.Add(',');
                    sql.Put(null, names[i]);
                }
                sql.Add(']');
                if (shard != null)
                {
                    // the IN clause with shard is normally fixed, don't need to be parameters
                    sql._("AND (shard IS NULL OR shard =")._(shard)._(")");
                }
                sql._(" LIMIT 1");

                if (dc.Query1(p => p.Set(lastid.Value)))
                {
                    var evt = dc.ToObject<Event>();

                    ac.SetHeader("ID", evt.id);
                    ac.SetHeader("Date", evt.time);
                    if (evt.type != null)
                    {
                        ac.SetHeader("Content-Type", evt.type);
                    }
                    ac.Reply(200);
                }
                else
                {
                    ac.Reply(204); // no content
                }
            }
        }

    }
}