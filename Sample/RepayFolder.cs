using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    [Ui("结款管理")]
    public class RepayFolder<V> : Folder where V : RepayVarFolder
    {
        static readonly Connector WcPay = new Connector("https://api.mch.weixin.qq.com");

        public RepayFolder(FolderContext fc) : base(fc)
        {
            CreateVar<ItemVarFolder>();
        }

        [User]
        [Ui]
        public void @default(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM repays WHERE shopid = @1 AND status < 4", p => p.Set(shopid)))
                {
                    ac.GiveFolderPage(this, 200, dc.ToList<Repay>());
                }
                else
                {
                    ac.GiveFolderPage(this, 200, (List<Repay>)null);
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
                ac.GivePaneForm(303, dc, (i, o) =>
                {
                    o.Put(nameof(name), name = i.GetString());
                    o.Put(nameof(age), age = i.GetInt());
                }); // see other
            }
        }

        [User]
        [Ui("生成结款单")]
        public void @new(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                string name;
                int age;
                dc.Execute("UPDATE items SET enabled = NOT enabled WHERE shopid = @1", p => p.Set(shopid));
                // ac.SetHeader();
                ac.GivePaneForm(303, dc, (i, o) =>
                {
                    o.Put(nameof(name), name = i.GetString());
                    o.Put(nameof(age), age = i.GetInt());
                }); // see other
            }
        }

        [User]
        [Ui("结款")]
        public async Task pay(ActionContext ac)
        {
            string shopid = ac[Parent];

            int id = ac.Query[nameof(id)];

            // <xml>
            // <mch_appid>wxe062425f740c30d8</mch_appid>
            // <mchid>10000098</mchid>
            // <nonce_str>3PG2J4ILTKCH16CQ2502SI8ZNMTM67VS</nonce_str>
            // <partner_trade_no>100000982014120919616</partner_trade_no>
            // <openid>ohO4Gt7wVPxIT1A9GjFaMYMiZY1s</openid>
            // <check_name>OPTION_CHECK</check_name>
            // <re_user_name>张三</re_user_name>
            // <amount>100</amount>
            // <desc>节日快乐!</desc>
            // <spbill_create_ip>10.2.3.10</spbill_create_ip>
            // <sign>C97BDBACF37622775366F38B629F45E3</sign>
            // </xml>
            XmlContent cont = new XmlContent();
            XElem resp = await WcPay.PostAsync<XElem>(null, "/mmpaymkttransfers/promotion/transfers", cont);

            using (var dc = ac.NewDbContext())
            {
                string name;
                int age;
                dc.Execute("UPDATE items SET enabled = NOT enabled WHERE shopid = @1", p => p.Set(shopid));
                // ac.SetHeader();
                ac.GivePaneForm(303, dc, (i, o) =>
                {
                    o.Put(nameof(name), name = i.GetString());
                    o.Put(nameof(age), age = i.GetInt());
                }); // see other
            }
        }

        [Ui("余额管理")]
        public void balance(ActionContext ac)
        {
        }
    }

    public class ShopRepayFolder : RepayFolder<ShopRepayVarFolder>
    {
        public ShopRepayFolder(FolderContext fc) : base(fc)
        {
        }
    }

    public class OpRepayFolder : RepayFolder<OpRepayVarFolder>
    {
        public OpRepayFolder(FolderContext fc) : base(fc)
        {
        }
    }

}