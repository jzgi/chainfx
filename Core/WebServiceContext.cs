namespace Greatbone.Core
{
	public struct WebServiceContext
	{
		public string Key { get; set; }

		public string StaticPath { get; set; }

		public IParent Parent { get; set; }

		public string Address { get; set; }

		public int Port { get; set; }

		public bool Debug{ get; set; }

		public WebService Service { get; set; }
	}
}