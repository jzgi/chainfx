using System;

namespace Greatbone.Core
{
	public struct WebServiceContext : IData
	{
		private string key;

		private string staticpath;

		public string Key { get; set; }

		public string StaticPath { get; set; }

		public string Host { get; set; }

		public int Port { get; set; }

		public IParent Parent { get; set; }

		public NetAddress WebAddress { get; set; }

		public NetAddress DbAddress { get; set; }

		public bool Debug { get; set; }

		public WebService Service { get; set; }


		public void From(IDataInput i)
		{
			i.Got(nameof(key), ref key);
		}

		public void To(IDataOutput o)
		{
		}
	}
}