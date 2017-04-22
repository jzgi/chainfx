using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// An order data object.
    ///
    public class Order : IData
    {
        // state
        public const int
            CREATED = 0,
            PAID = 1,
            PACKED = 2,
            SHIPPED = 4,
            ABORTED = 16,
            COMPLETED = 32;

        // status
        static readonly Opt<short> STATUS = new Opt<short>
        {
            [CREATED] = "新创建，等待付款",
            [PAID] = "已付款，等待处理",
            [PACKED] = "已备货",
            [SHIPPED] = "已派送，等待收货",
            [ABORTED] = "已撤销",
            [COMPLETED] = "已完成",
        };


        public static readonly Order Empty = new Order();

        internal int id;
        internal string shop; // shop name
        internal string shopid;
        internal string buy; // buyer name or nickname
        internal string buywx; // buyer openid
        internal string buytel; // buyer telephone
        internal string buydistr; // shop openid
        internal string buyaddr; // buyer shipping address
        internal List<OrderLine> detail;
        internal decimal total;
        internal string note;
        internal DateTime created; // time created

        internal string prepay_id;
        internal DateTime paid; // time paid

        internal string pack; // packer name
        internal string packtel;
        internal DateTime packed;

        internal string shipshopid;
        internal string ship; // packer name
        internal string shiptel;
        internal DateTime shipped;

        internal DateTime closed; // time completed or aborted

        internal short status;

        public void ReadData(IDataInput i, int proj = 0)
        {
            if (proj.Prime() && proj.Auto())
            {
                i.Get(nameof(id), ref id);
            }

            i.Get(nameof(shop), ref shop);
            i.Get(nameof(shopid), ref shopid);

            i.Get(nameof(buy), ref buy);
            i.Get(nameof(buywx), ref buywx);
            i.Get(nameof(buytel), ref buytel);
            i.Get(nameof(buydistr), ref buydistr);
            i.Get(nameof(buyaddr), ref buyaddr);
            if (proj.Detail())
            {
                i.Get(nameof(detail), ref detail);
            }
            i.Get(nameof(total), ref total);
            i.Get(nameof(note), ref note);
            i.Get(nameof(created), ref created);

            if (proj.Late())
            {
                i.Get(nameof(prepay_id), ref prepay_id);
                i.Get(nameof(paid), ref paid);

                i.Get(nameof(pack), ref pack);
                i.Get(nameof(packtel), ref packtel);
                i.Get(nameof(packed), ref packed);

                i.Get(nameof(shipshopid), ref shipshopid);
                i.Get(nameof(ship), ref ship);
                i.Get(nameof(shiptel), ref shiptel);
                i.Get(nameof(shipped), ref shipped);

                i.Get(nameof(closed), ref closed);
            }

            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            if (proj.Prime() && proj.Auto())
            {
                o.Put(nameof(id), id);
            }
            o.Begin("供应点");
            o.Put(nameof(shop), shop);
            o.Put(nameof(shopid), shopid);
            o.End();

            o.Put(nameof(buy), buy, label: "买家名称");
            o.Put(nameof(buywx), buywx);

            o.Begin("送货地址");
            o.Put(nameof(buytel), buytel);
            o.Put(nameof(buydistr), buydistr);
            o.Put(nameof(buyaddr), buyaddr);
            o.End();

            if (proj.Detail())
            {
                o.Put(nameof(detail), detail);
            }
            o.Put(nameof(total), total, label: "金额");
            o.Put(nameof(note), note, label: "附加说明");
            o.Put(nameof(created), created, label: "创建时间");

            if (proj.Late())
            {
                if (proj.PhaseA())
                {
                    o.Put(nameof(prepay_id), prepay_id);
                    o.Put(nameof(paid), paid);
                }
                if (proj.PhaseB())
                {
                    o.Put(nameof(pack), pack);
                    o.Put(nameof(packtel), packtel);
                    o.Put(nameof(packed), packed);
                }
                if (proj.PhaseC())
                {
                    o.Put(nameof(shipshopid), shipshopid);
                    o.Put(nameof(ship), ship);
                    o.Put(nameof(shiptel), shiptel);
                    o.Put(nameof(shipped), shipped);
                }
                if (proj.PhaseD())
                {
                    o.Put(nameof(closed), closed);
                }
            }
            o.Put(nameof(status), status, label: "状态", opt: STATUS);
        }

        public void AddItem(string item, short qty, decimal price, string note)
        {
            if (detail == null)
            {
                detail = new List<OrderLine>();
            }
            var orderln = detail.Find(o => o.item.Equals(item));
            if (orderln.item == null)
            {
                orderln = new OrderLine();
                detail.Add(orderln);
            }
        }
    }

    public struct OrderLine : IData
    {
        internal string item;
        internal short qty;
        internal string unit;
        internal decimal price;

        public decimal Subtotal => price * qty;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(item), ref item);
            i.Get(nameof(qty), ref qty);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(price), ref price);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(item), item, label: "品名");
            o.Begin("数量");
            o.Put(nameof(qty), qty);
            o.Put(nameof(unit), unit);
            o.End();
            o.Put(nameof(price), price, label: "单价");
        }

        public void AddQty(short qty)
        {
            this.qty += qty;
        }
    }
}