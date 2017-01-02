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

        public static void Set<D>(this HttpRequestMessage msg, D data) where D : IData
        {
            JsonContent cont = new JsonContent(true, true);
            cont.Put(null, data);
            msg.Content = cont;
        }

        public static void Set<D>(this HttpRequestMessage msg, D[] datas) where D : IData
        {
            JsonContent cont = new JsonContent(true, true);
            cont.Put(null, datas);
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
            XmlParse p = new XmlParse(bytes, bytes.Length);
            return (XElem)p.Parse();
        }

        public static async Task<D> GetDataObjAsync<D>(this HttpResponseMessage msg, byte z = 0) where D : IData, new()
        {
            byte[] bytes = await msg.Content.ReadAsByteArrayAsync();
            JsonParse p = new JsonParse(bytes, bytes.Length);
            JObj jobj = (JObj)p.Parse();
            return jobj.ToDataObj<D>(z);
        }

        public static async Task<D[]> GetDataArrAsync<D>(this HttpResponseMessage msg, byte z = 0) where D : IData, new()
        {
            byte[] bytes = await msg.Content.ReadAsByteArrayAsync();
            JsonParse p = new JsonParse(bytes, bytes.Length);
            JArr jarr = (JArr)p.Parse();
            return jarr.ToDataArr<D>(z);
        }

        public static async Task<byte[]> GetBytesSegAsync(this HttpResponseMessage msg)
        {
            return await msg.Content.ReadAsByteArrayAsync();
        }
    }
}