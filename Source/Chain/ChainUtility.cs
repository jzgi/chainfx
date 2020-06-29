using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SkyCloud.Chain
{
    public static class ChainUtility
    {
        static readonly Map<short, BlockDef> blockDefs = new Map<short, BlockDef>();


        public static void RegisterBlockDef(short id, string name,
            IConsensus cons,
            ISmartContract scon)
        {
        }

        public static string GetValue(this HttpHeaders headers, string name)
        {
            if (headers.TryGetValues(name, out var values))
            {
                string[] strs = values as string[];
                return strs?[0];
            }

            return null;
        }

        public static void CallAny(this ChainClient[] df)
        {
        }

        public static void CallAll(this ChainClient[] df)
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