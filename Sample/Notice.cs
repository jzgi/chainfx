using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
	public class Notice : ISerial
	{
		public int Id;

		public DateTime Opened;

		public string Title;

		public string Description;

		public int OffererId;

		public int AcceptorId;

		public int Term;

		public long ModifiedOn { get; set; }

		public string Key { get; }

		public void From(IReader r)
		{
			r.Read(nameof(Id), ref Id);
			r.Read(nameof(Title), ref Title);
			r.Read(nameof(OffererId), ref OffererId);
			r.Read(nameof(Id), ref Id);
			r.Read(nameof(Id), ref Id);
		}

		public void To(IWriter w)
		{
			w.Write(nameof(Id), Id);
			w.Write(nameof(Title), Title);
		}
	}
}