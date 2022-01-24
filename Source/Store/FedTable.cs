using System.Text;
using System.Threading.Tasks;
using SkyChain.Web;

namespace SkyChain.Store
{
    internal class FedTable : IKeyable<string>
    {
        readonly string table_name;

        readonly Map<string, FedColumn> customs = new Map<string, FedColumn>(16);

        FedColumn peer_;

        FedColumn id_;

        FedColumn coid_;

        FedColumn trace_;

        FedColumn phase_;

        FedColumn seq_;

        FedColumn cs_;

        FedColumn blockcs_;

        public FedTable(string table_name)
        {
            this.table_name = table_name;
        }

        public string Key => table_name;


        internal void AddColumn(FedColumn col)
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