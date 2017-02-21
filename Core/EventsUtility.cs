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
                                CONSTRAINT eu_pkey PRIMARY KEY (moniker)
                                ) WITH (OIDS=FALSE)"
            );

            dc.Execute(@"CREATE SEQUENCE IF NOT EXISTS evtq_id_seq 
                                INCREMENT 1 
                                MINVALUE 1 MAXVALUE 9223372036854775807 
                                START 1 CACHE 32 NO CYCLE
                                OWNED BY evtq.id"
            );

            dc.Execute(@"CREATE TABLE IF NOT EXISTS eq (
                                id int8 DEFAULT nextval('eq_id_seq'::regclass) NOT NULL,
                                name varchar(40),
                                shard varchar(20),
                                arg varchar(40),
                                time timestamp,
                                type varchar(40),
                                body bytea,
                                CONSTRAINT evtq_pkey PRIMARY KEY (id)
                                ) WITH (OIDS=FALSE)"
            );

        }

        static void Post(DbContext dc, string name, string shard, string arg, IContent content)
        {
            // convert message to byte buffer
            var byteas = new ArraySegment<byte>(content.ByteBuffer, 0, content.Size);
            dc.Execute("INSERT INTO eq (name, shard, arg, body) VALUES (@1, @2, @3, @4)", p =>
            {
                p.Set(name);
                p.Set(shard);
                p.Set(arg);
                p.Set(byteas);
            });
        }

        internal static FormMpContent PeekEq(DbContext dc, string[] names, string shard, long? lastid)
        {
            DbSql sql = dc.Sql("SELECT id, name, time, type, body FROM evtq WHERE id > @1 AND event IN [");
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
            sql._("LIMIT 120");

            if (dc.Query(p => p.Set(lastid.Value)))
            {
                FormMpContent cont = new FormMpContent(true, capacity: 1024 * 1024);
                while (dc.Next())
                {
                    long id = dc.GetLong();
                    string name = dc.GetString();
                    DateTime time = dc.GetDateTime();
                    string type = dc.GetString();
                    ArraySegment<byte> body = dc.GetBytesSeg();

                    // add an extension part
                    // cont.PutEvent(id, name, time, type, body);
                }
            }
            else
            {
            }
            return null;
        }
    }
}