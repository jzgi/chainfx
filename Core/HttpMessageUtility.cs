using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// A lot of convenient method for request/response operations.
    ///
    public static class HttpMessageUtility
    {

        public static void Set(this HttpRequestMessage msg, JObj jobj)
        {
            JsonContent cont = new JsonContent(true, true);
            jobj.Dump(cont);
            msg.Content = cont;
        }

        public static void Set(this HttpRequestMessage msg, JArr jobj)
        {
            JsonContent cont = new JsonContent(true, true);
            jobj.Dump(cont);
            msg.Content = cont;
        }

        public static void Set<D>(this HttpRequestMessage msg, D dat) where D : IData
        {
            JsonContent cont = new JsonContent(true, true);
            cont.Put(null, dat);
            msg.Content = cont;
        }

        public static void Set<D>(this HttpRequestMessage msg, D[] arr) where D : IData
        {
            JsonContent cont = new JsonContent(true, true);
            cont.Put(null, arr);
            msg.Content = cont;
        }

        //
        // response
        //

        public static int GetStatus(this HttpResponseMessage msg) => (int)msg.StatusCode;


        public static async Task<JObj> GetJObjAsync(this HttpResponseMessage msg)
        {
            byte[] bytes = await msg.Content.ReadAsByteArrayAsync();
            JsonParse p = new JsonParse(bytes, bytes.Length);
            return (JObj)p.Parse();
        }

        public static async Task<JArr> GetJArrAsync(this HttpResponseMessage msg)
        {
            byte[] bytes = await msg.Content.ReadAsByteArrayAsync();
            JsonParse p = new JsonParse(bytes, bytes.Length);
            return (JArr)p.Parse();
        }

        public static async Task<XElem> GetXElemAsync(this HttpResponseMessage msg)
        {
            byte[] bytes = await msg.Content.ReadAsByteArrayAsync();
            XmlParse p = new XmlParse(bytes, 0, bytes.Length);
            return (XElem)p.Parse();
        }

        public static async Task<D> GetObjectAsync<D>(this HttpResponseMessage msg, byte flags = 0) where D : IData, new()
        {
            byte[] bytes = await msg.Content.ReadAsByteArrayAsync();
            JsonParse p = new JsonParse(bytes, bytes.Length);
            JObj jobj = (JObj)p.Parse();
            return jobj.ToData<D>(flags);
        }

        public static async Task<D[]> GetArrayAsync<D>(this HttpResponseMessage msg, byte flags = 0) where D : IData, new()
        {
            byte[] bytes = await msg.Content.ReadAsByteArrayAsync();
            JsonParse p = new JsonParse(bytes, bytes.Length);
            JArr jarr = (JArr)p.Parse();
            return jarr.ToDatas<D>(flags);
        }

        public static async Task<byte[]> GetBytesSegAsync(this HttpResponseMessage msg)
        {
            return await msg.Content.ReadAsByteArrayAsync();
        }
    }
}