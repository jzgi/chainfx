using System;

namespace Greatbone.Core
{
    /// <summary>
    /// A event queue pertaining to a certain event client.
    /// </summary>
    public class EventQueue : IRollable
    {
        internal const string
            X_EVENT = "X-Event",
            X_SHARD = "X-Shard",
            X_ARG = "X-Arg",
            X_ID = "X-ID";

        const int CAPACITY = 120;

        readonly string name;

        readonly Event[] elements;

        int head;

        int count;

        //
        // keeping last query condition

        string lastevent;

        string lastshard;

        long evtid;

        internal EventQueue(string name)
        {
            this.name = name;
            elements = new Event[CAPACITY];
        }

        public string Name => name;

        public void Poll(ActionContext ac)
        {
            string @event = ac.Header(X_EVENT); // maybe a number of names
            string shard = ac.Header(X_SHARD);
            long? id = ac.HeaderLong(X_ID);

            lock (this)
            {
                if (@event != lastevent || shard != lastshard || id != evtid)
                {
                    Clear();
                }

                // try in-memory
                if (count == 0) // not in memory
                {
                    using (var dc = ac.NewDbContext())
                    {
                        DbSql sql = dc.Sql("SELECT * FROM EVTQ WHERE id > @1 AND name IN (");
                        string[] names = @event.Split(',');
                        for (int i = 0; i < names.Length; i++)
                        {
                            if (i > 0) sql.Add(',');
                            sql.Put(null, names[i]); // quoted name
                        }
                        sql.Add(')');
                        if (shard != null)
                        {
                            sql._("AND (shard IS NULL OR shard =").Put(null, shard)._(")");
                        }
                        sql._("LIMIT @2");

                        if (dc.Query(p => p.Set(id.Value).Set(CAPACITY)))
                        {
                            // load & cache into memory
                            while (dc.Next())
                            {
                                elements[count++] = dc.ToObject<Event>();
                            }
                        }
                        else
                        {
                            ac.Give(204); // no content
                            return;
                        }
                    }
                }

                // remove & return the head
                Event e = elements[head++];
                count--;

                // set headers
                ac.SetHeader(X_EVENT, e.name);
                ac.SetHeader(X_SHARD, e.shard);
                ac.SetHeader(X_ARG, e.arg);
                ac.SetHeader(X_ID, e.id);

                // set content, if any
                IContent cont = null;
                if (e.type != null)
                {
                    cont = new StaticContent(e.body) {Type = e.type};
                }
                ac.Give(200, cont);

                // keep for state validation of next poll 
                lastevent = e.name;
                lastshard = e.shard;
                evtid = e.id;
            }
        }

        public void Clear()
        {
            lock (this)
            {
                head = 0;
                count = 0;
            }
        }

        //
        // static
        //

        internal static void Setup(Service service, Roll<Client> clients)
        {
            using (var dc = service.NewDbContext())
            {
                // create database objects

                dc.Execute(@"
                    CREATE TABLE IF NOT EXISTS evtu (
                        peerid varchar(20),
                        evtid int8,
                        CONSTRAINT evtu_pkey PRIMARY KEY (peerid)
                    ) WITH (OIDS=FALSE);

                    CREATE SEQUENCE IF NOT EXISTS evtq_id_seq 
                        INCREMENT 1 
                        MINVALUE 1 MAXVALUE 9223372036854775807 
                        START 1 CACHE 100 NO CYCLE;

                    CREATE TABLE IF NOT EXISTS evtq (
                        id int8 DEFAULT nextval('evtq_id_seq'::regclass) NOT NULL,
                        name varchar(40),
                        shard varchar(20),
                        arg varchar(40),
                        type varchar(40),
                        body bytea,
                        time timestamp,
                        CONSTRAINT evtq_pkey PRIMARY KEY (id)
                    ) WITH (OIDS=FALSE)"
                );

                // init records for each peerid 

                for (int i = 0; i < clients.Count; i++)
                {
                    Client cli = clients[i];
                    if (dc.Query1("SELECT evtid FROM evtu WHERE peerid = @1", p => p.Set(cli.Name)))
                    {
                        dc.Let(out cli.evtid);
                    }
                    else
                    {
                        dc.Execute("INSERT INTO evtu (peerid, evtid) VALUES (@1, @2)", p => p.Set(cli.Name).Set(cli.evtid));
                    }
                }
            }
        }

        static void Post(DbContext dc, string name, string shard, string arg, IContent content)
        {
            if (content == null)
            {
                dc.Execute("INSERT INTO evtq (name, shard, arg) VALUES (@1, @2, @3)", p => { p.Set(name).Set(shard).Set(arg); });
            }
            else
            {
                var body = new ArraySegment<byte>(content.ByteBuffer, 0, content.Size);
                dc.Execute("INSERT INTO evtq (name, shard, arg, type, body) VALUES (@1, @2, @3, @4, @5)", p => { p.Set(name).Set(shard).Set(arg).Set(content.Type).Set(body); });
            }
        }
    }
}