using System;
using System.IO;

namespace Greatbone.Core
{
	public class WebServiceBuilder
	{
		public string Key { get; set; }

		public string StaticPath { get; set; }

		public string Host { get; set; }

		public int Port { get; set; }

		public string QHost { get; set; }

		public int QPort { get; set; }

		public string DbHost { get; set; }

		public int DbPort { get; set; }

		public string DbUsername { get; set; }

		public string DbPassword { get; set; }


		internal IParent Parent { get; set; }

		public bool Debug { get; set; }

		public WebService Service { get; set; }


		public WebServiceBuilder()
		{
		}


		public WebServiceBuilder(string file)
		{
		}
	}
}