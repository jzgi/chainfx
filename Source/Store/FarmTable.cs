using System.Text;
using System.Threading.Tasks;
using SkyChain.Web;

namespace SkyChain.Store
{
    internal class FarmTable : IKeyable<string>
    {
        readonly string table_name;

        readonly Map<string, DbColumn> customs = new Map<string, DbColumn>(16);

        DbColumn peer_;

        DbColumn id_;

        DbColumn coid_;

        DbColumn trace_;

        DbColumn phase_;

        DbColumn seq_;

        DbColumn cs_;

        DbColumn blockcs_;

        public FarmTable(string table_name)
        {
            this.table_name = table_name;
        }

        public string Key => table_name;


        internal void AddColumn(DbColumn col)
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