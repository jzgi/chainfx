using System;

namespace Greatbone.Core
{
    static class EventsUtility
    {
        internal static void CreateEq(DbContext dc)
        {
            dc.Execute(@"CREATE TABLE IF NOT EXISTS evtu (
                                moniker varchar(20),
                                lastid int8,
                                CONSTRAINT evtu_pkey PRIMARY KEY (moniker)
                                ) WITH (OIDS=FALSE)"
            );

            dc.Execute(@"CREATE SEQUENCE IF NOT EXISTS evtq_id_seq 
                                INCREMENT 1 
                                MINVALUE 1 MAXVALUE 9223372036854775807 
                                START 1 CACHE 100 NO CYCLE
                                OWNED BY evtq.id"
            );

            dc.Execute(@"CREATE TABLE IF NOT EXISTS evtq (
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

        }

        static void Post(DbContext dc, string name, string shard, string arg, IContent content)
        {
            if (content == null)
            {
                dc.Execute("INSERT INTO evtq (name, shard, arg) VALUES (@1, @2, @3)", p =>
                {
                    p.Set(name).Set(shard).Set(arg);
                });
            }
            else
            {
                var body = new ArraySegment<byte>(content.ByteBuffer, 0, content.Size);
                dc.Execute("INSERT INTO evtq (name, shard, arg, type, body) VALUES (@1, @2, @3, @4, @5)", p =>
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