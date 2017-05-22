using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// An order data object.
    ///
    public class Order : IData
    {
        public const short
            ID = 0x0001,
            WX = 0x0002,
            DETAIL = 0x0020,
            LATE = 0x0010;

        // status
        public const short CREATED = 0, ACCEPTED = 1, ABORTED = 3, COMPLETED = 5;

        // status
        static readonly Opt<short> STATUS = new Opt<short>
        {
            [CREATED] = "购物车",
            [ACCEPTED] = "已接受，在处理",
            [ABORTED] = "已撤销",
            [COMPLETED] = "已完成",
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
        internal string comment;
        internal decimal total; // receivable
        internal decimal cash; // amount recieved

        internal string partnerid; // delegate shopid
        internal DateTime accepted; // when cash received or forcibly accepted
        internal string abortion; // time aborted
        internal DateTime closed; // time completed
        internal short status;

        public void ReadData(IDataInput i, short proj = 0)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
            }
            i.Get(nameof(created), ref created);
            i.Get(nameof(shop), ref shop);
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(buyer), ref buyer);
            if ((proj & WX) == WX)
            {
                i.Get(nameof(wx), ref wx);
            }
            i.Get(nameof(city), ref city);
            i.Get(nameof(distr), ref distr);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(tel), ref tel);
            if ((proj & DETAIL) == DETAIL)
            {
                i.Get(nameof(detail), ref detail);
            }
            i.Get(nameof(comment), ref comment);
            i.Get(nameof(total), ref total);

            if ((proj & LATE) == LATE)
            {
                i.Get(nameof(cash), ref cash);
                i.Get(nameof(accepted), ref accepted);
                i.Get(nameof(partnerid), ref partnerid);
                i.Get(nameof(abortion), ref abortion);
                i.Get(nameof(closed), ref closed);
            }

            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id, "订单编号");
            }
            o.Put(nameof(created), created, "创建时间");

            o.Group("商家");
            o.Put(nameof(shop), shop);
            o.Put(nameof(shopid), shopid);
            o.UnGroup();

            o.Put(nameof(buyer), buyer, "买家");
            if ((proj & WX) == WX)
            {
                o.Put(nameof(wx), wx);
            }
            o.Group("收货地址");
            o.Put(nameof(city), city);
            o.Put(nameof(distr), distr);
            o.Put(nameof(addr), addr);
            o.UnGroup();
            o.Put(nameof(tel), tel, "联系电话");

            if ((proj & DETAIL) == DETAIL)
            {
                o.Put(nameof(detail), detail);
            }
            o.Put(nameof(comment), comment, "附加说明");
            o.Put(nameof(total), total, "应付金额", '¥');

            if ((proj & LATE) == LATE)
            {
                o.Put(nameof(cash), cash, "实收金额", '¥');
                o.Put(nameof(accepted), accepted, "实收时间");
                o.Put(nameof(partnerid), partnerid);
                o.Put(nameof(abortion), abortion);
                o.Put(nameof(closed), closed);
            }
            o.Put(nameof(status), status, "状态", STATUS);
        }

        public void AddItem(string item, short qty, string unit, decimal price)
        {
            if (detail == null)
            {
                detail = new[] {new OrderLine() {name = item, qty = qty, unit = unit, price = price}};
            }
            var orderln = detail.Find(o => o.name.Equals(item));
            if (orderln != null)
            {
                orderln.qty += qty;
            }
            else
            {
                detail = detail.Add(new OrderLine() {name = item, qty = qty, unit = unit, price = price});
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

        public void ReadData(IDataInput i, short proj = 0)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(qty), ref qty);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(price), ref price);
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
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