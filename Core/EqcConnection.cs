using System;
using System.Net.Http;

namespace Greatbone.Core
{
	public class EqcConnection
	{
		string address;

		HttpClient client;

		private bool status;

		// tick count
		private int lastConnect;

		public async void GetAsync()
		{
			using (client = new HttpClient())
			{
				client.BaseAddress = new Uri("");
				client.DefaultRequestHeaders.Add("Range", "");
				HttpResponseMessage resp = await client.GetAsync("");
				if (resp.IsSuccessStatusCode)
				{
				}
			}
		}


	}
}