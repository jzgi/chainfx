using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    [Ui("付款管理")]
    public class PayFolder : Folder
    {
        public PayFolder(FolderContext fc) : base(fc)
        {
            CreateVar<ItemVarFolder>();
        }

        public async Task notify(ActionContext ac)
        {
            XElem xe = await ac.ReadAsync<XElem>();
            string appid = xe[nameof(appid)];
            string mch_id = xe[nameof(mch_id)];
            string openid = xe[nameof(openid)];
            string nonce_str = xe[nameof(nonce_str)];
            string sign = xe[nameof(sign)];
            string result_code = xe[nameof(result_code)];

            string bank_type = xe[nameof(bank_type)];
            string total_fee = xe[nameof(total_fee)]; // 订单总金额单位分
            string cash_fee = xe[nameof(cash_fee)]; // 支付金额单位分
            string transaction_id = xe[nameof(transaction_id)]; // 微信支付订单号
            string out_trade_no = xe[nameof(out_trade_no)]; // 商户订单号
            string time_end = xe[nameof(time_end)]; // 支付完成时间

        }

        public void lst(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM items WHERE shopid = @1 AND enabled", p => p.Set(shopid)))
                {
                    ac.Give(200, dc.Dump());
                }
                else
                {
                    ac.Give(204);
                }
            }
        }

        [User]
        [Ui]
        public void @default(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status < 4", p => p.Set(shopid)))
                {
                    ac.GiveFolderPage(this, 200, dc.ToList<Item>());
                }
                else
                {
                    ac.GiveFolderPage(this, 200, (List<Item>)null);
                }
            }
        }

        [User]
        public void _cat_(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                string name;
                int age;
                dc.Execute("UPDATE items SET enabled = NOT enabled WHERE shopid = @1", p => p.Set(shopid));
                // ac.SetHeader();
                ac.GiveModalForm(303, dc, (i, o) =>
                {
                    o.Put(nameof(name), name = i.GetString());
                    o.Put(nameof(age), age = i.GetInt());
                }); // see other
            }
        }
    }
}