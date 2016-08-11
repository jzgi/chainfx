using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
	public class Job : ISerial
	{
		int id;

		DateTime opened;

		string title;

		string description;

		int offererId;

		int acceptorId;

		int term;


		public void From(IReader r)
		{
			r.Read(nameof(id), ref id);
			r.Read(nameof(title), ref title);
			r.Read(nameof(offererId), ref offererId);
			r.Read(nameof(id), ref id);
			r.Read(nameof(id), ref id);
		}

		public void To(IWriter w)
		{
			w.Write(nameof(id), id);
			w.Write(nameof(title), title);
		}


		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public DateTime Opened
		{
			get { return opened; }
			set { opened = value; }
		}

		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		public int AcceptorId
		{
			get { return acceptorId; }
			set { acceptorId = value; }
		}

		public int Term
		{
			get { return term; }
			set { term = value; }
		}
	}
}