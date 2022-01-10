using System.Text;
using System.Threading.Tasks;
using SkyChain.Web;

namespace SkyChain.Chain
{
    internal class ChainTable : IKeyable<string>
    {
        readonly string table_name;

        readonly Map<string, ChainColumn> customs = new Map<string, ChainColumn>(16);

        ChainColumn peer_;

        ChainColumn id_;

        ChainColumn coid_;

        ChainColumn ender_; // arhive or abort

        ChainColumn seq_;

        ChainColumn cs_;

        ChainColumn blockcs_;

        public ChainTable(string table_name)
        {
            this.table_name = table_name;
        }

        public string Key => table_name;


        internal void AddColumn(ChainColumn col)
        {
            customs.Add(col);
        }

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
                for (int i = 0; i < customs.Count; i++)
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
                    for (int i = 0; i < customs.Count; i++)
                    {
                    }

                    cnt._OBJ();
                }

                cnt._ARR();
            }

            return cnt;
        }
    }
}