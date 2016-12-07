using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    public static class WebClientUtility
    {

        public static void CallAny(WebClient[] df)
        {

        }
        public static void CallAll(WebClient[] df)
        {

        }

        public static async void CallAll(Task<HttpResponseMessage>[] requests, Action<HttpResponseMessage> a)
        {
            HttpResponseMessage[] results = await Task.WhenAll(requests);
            for (int i = 0; i < results.Length; i++)
            {
                a(results[i]);
            }
        }
    }
}