using System;
using SkyChain;

namespace SkyChain.Chain
{
    public class DbTyp : IKeyable<uint>
    {
        // system base types
        //

        static readonly Map<uint, DbTyp> BASE = new Map<uint, DbTyp>()
        {
            new DbTyp(16, "BOOL")
            {
                Converter = (name, src, snk) =>
                {
                    bool v = false;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(18, "CHAR")
            {
                Converter = (name, src, snk) =>
                {
                    char v = (char) 0;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(21, "SMALLINT")
            {
                Converter = (name, src, snk) =>
                {
                    short v = 0;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(23, "INT")
            {
                Converter = (name, src, snk) =>
                {
                    int v = 0;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(20, "BIGINT")
            {
                Converter = (name, src, snk) =>
                {
                    long v = 0;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(790, "MONEY")
            {
                Converter = (name, src, snk) =>
                {
                    decimal v = 0;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(700, "FLOAT")
            {
                Converter = (name, src, snk) =>
                {
                    float v = 0;
                    // src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(701, "DOUBLE")
            {
                Converter = (name, src, snk) =>
                {
                    double v = 0;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(1700, "NUMERIC")
            {
                Converter = (name, src, snk) =>
                {
                    decimal v = default;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(1082, "DATE")
            {
                Converter = (name, src, snk) =>
                {
                    DateTime v = default;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(1083, "TIME")
            {
                Converter = (name, src, snk) =>
                {
                    DateTime v = default;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(1114, "TIMESTAMP")
            {
                Converter = (name, src, snk) =>
                {
                    DateTime v = default;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(1184, "TIMESTAMPTZ")
            {
                Converter = (name, src, snk) =>
                {
                    DateTime v = default;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(1266, "TIMETZ")
            {
                Converter = (name, src, snk) =>
                {
                    DateTime v = default;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(2950, "UUID")
            {
                Converter = (name, src, snk) =>
                {
                    Guid v = default;
                    // src.Get(name, ref v);
                    // snk.Put(name, v);
                }
            },
            new DbTyp(1043, "VARCHAR")
            {
                Converter = (name, src, snk) =>
                {
                    string v = null;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(25, "TEXT")
            {
                Converter = (name, src, snk) =>
                {
                    string v = null;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(114, "JSON")
            {
                Converter = (name, src, snk) =>
                {
                    string v = null;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(142, "XML")
            {
                Converter = (name, src, snk) =>
                {
                    string v = null;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(3802, "JSONB")
            {
                Converter = (name, src, snk) =>
                {
                    string v = null;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(17, "BYTEA")
            {
                Converter = (name, src, snk) =>
                {
                    byte[] v = null;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(1000, "BOOL[]")
            {
                Converter = (name, src, snk) =>
                {
                    bool[] v = null;
                    // src.Get(name, ref v);
                    // snk.Put(name, v);
                }
            },
            new DbTyp(1002, "CHAR[]")
            {
                Converter = (name, src, snk) =>
                {
                    char[] v = null;
                    // src.Get(name, ref v);
                    // snk.Put(name, v);
                }
            },
            new DbTyp(1005, "SMALLINT[]")
            {
                Converter = (name, src, snk) =>
                {
                    short v = 0;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(1007, "INT[]")
            {
                Converter = (name, src, snk) =>
                {
                    int v = 0;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(1016, "BIGINT[]")
            {
                Converter = (name, src, snk) =>
                {
                    long v = 0;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(791, "MONEY[]")
            {
                Converter = (name, src, snk) =>
                {
                    decimal v = 0;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
            new DbTyp(1021, "FLOAT[]")
            {
                Converter = (name, src, snk) =>
                {
                    float[] v = null;
                    // src.Get(name, ref v);
                    // snk.Put(name, v);
                }
            },
            new DbTyp(1022, "DOUBLE[]")
            {
                Converter = (name, src, snk) =>
                {
                    double[] v = null;
                    // src.Get(name, ref v);
                    // snk.Put(name, v);
                }
            },
            new DbTyp(1015, "VARCHAR[]")
            {
                Converter = (name, src, snk) =>
                {
                    string[] v = null;
                    src.Get(name, ref v);
                    snk.Put(name, v);
                }
            },
        };

        readonly uint oid;

        readonly string name;

        internal readonly uint arrayoid;

        // columns if this is a composite type
        readonly Map<string, DbColumn> columns = new Map<string, DbColumn>(64);


        internal DbTyp(uint oid, string name, uint arrayoid = 0)
        {
            this.oid = oid;
            this.name = name;
            this.arrayoid = arrayoid;
        }

        internal void AddColumn(DbColumn column)
        {
            columns.Add(column);
        }

        public DbTyp ElementType { get; internal set; }

        public Action<string, ISource, ISink> Converter { get; internal set; }

        public uint Key => oid;

        public string Name => name;

        public static DbTyp GetBaseType(uint oid) => BASE[oid];
    }
}