using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// An order data object.
    ///
    public class Order : IData
    {
        public const ushort
            ID = 0x0001,
            BASIC = 0x0002,
            BASIC_WX = 0x006,
            BASIC_DETAIL = 0x000A,
            CASH = 0x0100,
            FLOW = 0x0200;

        // status
        public const short CREATED = 0, ACCEPTED = 1, ABORTED = 3, SHIPPED = 5, RECKONED = 7;

        // status
        static readonly Opt<short> STATUS = new Opt<short>
        {
            [CREATED] = "购物车/待付款",
            [ACCEPTED] = "已接受/在处理",
            [ABORTED] = "已撤销/关闭",
            [SHIPPED] = "买家已确收/未清算",
            [RECKONED] = "平台已清算/完成",
        };


        public static readonly Order Empty = new Order();

        internal long id;
        internal string shop; // shop name
        internal string shopid;
        internal DateTime created; // time created
        internal string buyer; // nuyer name
        internal string wx; // buyer weixin openid
        internal string city; // city
        internal string distr; // disrict
        internal string addr; // address
        internal string tel; // mobile number
        internal OrderLine[] detail;
        internal string note;
        internal decimal total; // receivable
        internal decimal cash; // amount recieved

        internal DateTime accepted; // when cash received or forcibly accepted
        internal string coshopid; // delegate shopid
        internal string abortion; // reason
        internal DateTime aborted; // time aborted
        internal DateTime shipped; // time shipped
        internal short status;

        public void Read(IDataInput i, ushort proj = 0x00ff)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
            }
            if ((proj & BASIC) == BASIC)
            {
                i.Get(nameof(created), ref created);
                i.Get(nameof(shop), ref shop);
                i.Get(nameof(shopid), ref shopid);
                i.Get(nameof(buyer), ref buyer);
                if ((proj & BASIC_WX) == BASIC_WX)
                {
                    i.Get(nameof(wx), ref wx);
                }
                i.Get(nameof(city), ref city);
                i.Get(nameof(distr), ref distr);
                i.Get(nameof(addr), ref addr);
                i.Get(nameof(tel), ref tel);
                if ((proj & BASIC_DETAIL) == BASIC_DETAIL)
                {
                    i.Get(nameof(detail), ref detail);
                }
                i.Get(nameof(note), ref note);
                i.Get(nameof(total), ref total);
            }
            if ((proj & CASH) == CASH)
            {
                i.Get(nameof(cash), ref cash);
            }
            if ((proj & FLOW) == FLOW)
            {
                i.Get(nameof(accepted), ref accepted);
                i.Get(nameof(coshopid), ref coshopid);
                i.Get(nameof(abortion), ref abortion);
                i.Get(nameof(aborted), ref aborted);
                i.Get(nameof(shipped), ref shipped);
            }
            i.Get(nameof(status), ref status);
        }

        public void Write<R>(IDataOutput<R> o, ushort proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id, "订单编号");
            }
            if ((proj & BASIC) == BASIC)
            {
                o.Put(nameof(created), created, "创建时间");
                o.Group("商家");
                o.Put(nameof(shop), shop);
                o.Put(nameof(shopid), shopid);
                o.UnGroup();
                o.Put(nameof(buyer), buyer, "买家");
                if ((proj & BASIC_WX) == BASIC_WX)
                {
                    o.Put(nameof(wx), wx);
                }
                o.Group("收货地址");
                o.Put(nameof(city), city);
                o.Put(nameof(distr), distr);
                o.Put(nameof(addr), addr);
                o.UnGroup();
                o.Put(nameof(tel), tel, "联系电话");
                if ((proj & BASIC_DETAIL) == BASIC_DETAIL)
                {
                    o.Put(nameof(detail), detail);
                }
                o.Put(nameof(note), note, "附加说明");
                o.Put(nameof(total), total, "应付金额", '¥');
            }
            if ((proj & CASH) == CASH)
            {
                o.Put(nameof(cash), cash, "实收金额", '¥');
            }
            if ((proj & FLOW) == FLOW)
            {
                o.Put(nameof(accepted), accepted, "接受时间");
                o.Put(nameof(coshopid), coshopid, "受托商家");
                o.Group("撤销");
                o.Put(nameof(abortion), abortion);
                o.Put(nameof(aborted), aborted);
                o.UnGroup();
                o.Put(nameof(shipped), shipped, "确收时间");
            }
            o.Put(nameof(status), status, "订单状态", STATUS);
        }

        public void AddItem(string item, short qty, string unit, decimal price)
        {
            if (detail == null)
            {
                detail = new[] { new OrderLine() { name = item, qty = qty, unit = unit, price = price } };
            }
            var orderln = detail.Find(o => o.name.Equals(item));
            if (orderln != null)
            {
                orderln.qty += qty;
            }
            else
            {
                detail = detail.AddOf(new OrderLine() { name = item, qty = qty, unit = unit, price = price });
            }
        }

        public void Sum()
        {
            if (detail != null)
            {
                decimal sum = 0;
                for (int i = 0; i < detail.Length; i++)
                {
                    sum += detail[i].qty * detail[i].price;
                }
                total = sum;
            }
        }

        public void SetLineQty(string name, short qty)
        {
            var ln = detail.Find(x => x.name == name);
            if (ln != null)
            {
                ln.qty = qty;
            }
        }

        public void RemoveLine(string name)
        {
            detail = detail.RemovedOf(x => x.name == name);
        }
    }

    public class OrderLine : IData
    {
        internal string name;
        internal short qty;
        internal string unit;
        internal decimal price;

        public decimal Subtotal => price * qty;

        public void Read(IDataInput i, ushort proj = 0x00ff)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(qty), ref qty);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(price), ref price);
        }

        public void Write<R>(IDataOutput<R> o, ushort proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name, "品名");
            o.Group("数量");
            o.Put(nameof(qty), qty);
            o.Put(nameof(unit), unit);
            o.UnGroup();
            o.Put(nameof(price), price, "单价");
        }

        public void AddQty(short qty)
        {
            this.qty += qty;
        }
    }
}