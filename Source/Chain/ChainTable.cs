using System.Text;
using System.Threading.Tasks;
using SkyChain;
using SkyChain.Web;

namespace SkyChain.Chain
{
    public class ChainTable
    {
        /// <summary>
        /// The database source that governs this view set.
        /// </summary>
        public DbSource Source { get; internal set; }

        readonly uint oid;

        readonly string name;

        readonly string check_option;


        readonly Map<string, DbColumn> columns = new Map<string, DbColumn>(64);

        internal ChainTable(DbContext s)
        {
            s.Get(nameof(oid), ref oid);
            s.Get(nameof(name), ref name);

            const string BEGIN = "current_setting('";
            const string END = "'";
            int p = 0;

            s.Get(nameof(check_option), ref check_option);

            bool pk = false;
            bool updatable = false;
            bool insertable = false;
            s.Get(nameof(pk), ref pk);
            s.Get(nameof(updatable), ref updatable);
            s.Get(nameof(insertable), ref insertable);
        }

        public uint Oid => oid;

        internal void AddColumn(DbColumn column)
        {
            columns.Add(column);
        }

        public bool Identifiable => false;

        public async Task OperateAsync(WebContext wc, string method, string[] vars, string subscript)
        {
            var sql = new StringBuilder();

            // set vars as session variables
        }

        JsonContent Dump(DbContext dc, bool single)
        {
            var cnt = new JsonContent(true, 8192);
            if (single)
            {
                cnt.OBJ_();
                for (int i = 0; i < columns.Count; i++)
                {
                }

                cnt._OBJ();
            }
            else
            {
                cnt.ARR_();
                while (dc.Next())
                {
                    cnt.OBJ_();
                    for (int i = 0; i < columns.Count; i++)
                    {
                    }

                    cnt._OBJ();
                }

                cnt._ARR();
            }

            return cnt;
        }

        internal void Describe(HtmlContent h)
        {
            h.T("<article style=\"border: 1px solid silver; padding: 8px;\">");

            h.T("<header>");
            if (Identifiable)
            {
                h.T("[id]");
            }

            h.T("</code>");
            h.T("</header>");

            h.T("<ul>");
            for (int i = 0; i < columns.Count; i++)
            {
                h.T("<li>");
                h.T("</li>");
            }

            h.T("</ul>");

            // methods and roles
            //

            h.T("<ul>");

            h.T("</ul>");

            h.T("</article>");
        }
    }
}