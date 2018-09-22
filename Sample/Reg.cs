using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// A region of operation that has its own weixin official acount.
    /// </summary>
    public class Reg : IData, IKeyable<string>
    {
        public const string WXAUTH = "wxauth";

        public static readonly Reg Empty = new Reg();

        static readonly Client Connector = new Client("https://api.weixin.qq.com");

        Client WCPay;

        internal string id;
        internal string name;
        internal string appid;
        internal string appsecret;
        internal string mchid;
        internal string noncestr;
        internal string spbillcreateip;
        internal string key;


        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(appid), ref appid);
            s.Get(nameof(appsecret), ref appsecret);
            s.Get(nameof(mchid), ref mchid);
            s.Get(nameof(noncestr), ref noncestr);
            s.Get(nameof(spbillcreateip), ref spbillcreateip);
            s.Get(nameof(key), ref key);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(appid), appid);
            s.Put(nameof(appsecret), appsecret);
            s.Put(nameof(mchid), mchid);
            s.Put(nameof(noncestr), noncestr);
            s.Put(nameof(spbillcreateip), spbillcreateip);
            s.Put(nameof(key), key);
        }


        public string Key => id;

        public override string ToString() => name;

        // apiclient_cert.p12
        internal void InitWCPay(string p12file)
        {
            if (p12file != null)
            {
                // load and init client certificate
                var handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual
                };
                X509Certificate2 cert = new X509Certificate2(p12file, mchid, X509KeyStorageFlags.MachineKeySet);
                handler.ClientCertificates.Add(cert);
                WCPay = new Client(handler)
                {
                    BaseAddress = new Uri("https://api.mch.weixin.qq.com")
                };
            }
        }

        internal string accessToken;

        private int tick;

        public async Task<string> GetAccessTokenAsync()
        {
            int now = Environment.TickCount;
            if (accessToken == null || now < tick || now - tick > 3600000)
            {
                JObj jo = await Connector.GetAsync<JObj>("/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + appsecret, null);
                string access_token = jo?[nameof(access_token)];
                accessToken = access_token;
                tick = now;
            }
            return accessToken;
        }

        public void GiveRedirectWeiXinAuthorize(WebContext wc, string listenAddr)
        {
            string redirect_url = WebUtility.UrlEncode(listenAddr + wc.Uri);
            wc.SetHeader("Location", "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + appid + "&redirect_uri=" + redirect_url + "&response_type=code&scope=snsapi_base&state=wxauth#wechat_redirect");
            wc.Give(303);
        }

        public async Task<(string access_token, string openid)> GetAccessorAsync(string code)
        {
            string url = "/sns/oauth2/access_token?appid=" + appid + "&secret=" + appsecret + "&code=" + code + "&grant_type=authorization_code";
            JObj jo = await Connector.GetAsync<JObj>(url, null);
            if (jo == null)
            {
                return default((string, string));
            }
            string access_token = jo[nameof(access_token)];
            if (access_token == null)
            {
                return default((string, string));
            }
            string openid = jo[nameof(openid)];
            return (access_token, openid);
        }

        public async Task<User> GetUserInfoAsync(string access_token, string openid)
        {
            JObj jo = await Connector.GetAsync<JObj>("/sns/userinfo?access_token=" + access_token + "&openid=" + openid + "&lang=zh_CN", null);
            string nickname = jo[nameof(nickname)];
            string city = jo[nameof(city)];
            return new User {wx = openid, name = nickname};
        }

        static readonly DateTime EPOCH = new DateTime(1970, 1, 1);

        public static long NowMillis => (long) (DateTime.Now - EPOCH).TotalMilliseconds;

        public IContent BuildPrepayContent(string prepay_id)
        {
            string package = "prepay_id=" + prepay_id;
            string timeStamp = ((int) (DateTime.Now - EPOCH).TotalSeconds).ToString();
            JObj jo = new JObj
            {
                {"appId", appid},
                {"nonceStr", noncestr},
                {"package", package},
                {"signType", "MD5"},
                {"timeStamp", timeStamp}
            };
            jo.Add("paySign", Sign(jo, "paySign"));
            return jo.Dump();
        }

        public async Task<(string ticket, string url)> PostQrSceneAsync(int uid)
        {
            var jc = new JsonContent(true);
            jc.OBJ_();
            jc.Put("expire_seconds", 604800);
            jc.Put("action_name", "QR_SCENE");
            jc.OBJ_("action_info");
            jc.OBJ_("scene");
            jc.Put("scene_id", uid);
            jc._OBJ();
            jc._OBJ();
            jc._OBJ();
            var (_, jo) = await Connector.PostAsync<JObj>("/cgi-bin/qrcode/create?access_token=" + await GetAccessTokenAsync(), jc);
            return (jo["ticket"], jo["url"]);
        }

        public async Task PostSendAsync(string openid, string text)
        {
            var jc = new JsonContent(true);
            jc.OBJ_();
            jc.Put("touser", openid);
            jc.Put("msgtype", "text");
            jc.OBJ_("text");
            jc.Put("content", text);
            jc._OBJ();
            jc._OBJ();
            await Connector.PostAsync<JObj>("/cgi-bin/message/custom/send?access_token=" + await GetAccessTokenAsync(), jc);
        }

        public async Task PostSendAsync(string openid, string title, string descr, string url, string picurl = null)
        {
            var jc = new JsonContent(true);
            jc.OBJ_();
            jc.Put("touser", openid);
            jc.Put("msgtype", "news");
            jc.OBJ_("news").ARR_("articles").OBJ_();
            jc.Put("title", title);
            jc.Put("description", descr);
            jc.Put("url", url);
            jc.Put("picurl", picurl);
            jc._OBJ()._ARR()._OBJ();
            jc._OBJ();
            await Connector.PostAsync<JObj>("/cgi-bin/message/custom/send?access_token=" + await GetAccessTokenAsync(), jc);
        }

        public async Task<string> PostTransferAsync(int id, string openid, string username, decimal cash, string desc)
        {
            XElem x = new XElem("xml")
            {
                {"amount", ((int) (cash * 100)).ToString()},
                {"check_name", "FORCE_CHECK"},
                {"desc", desc},
                {"mch_appid", appid},
                {"mchid", mchid},
                {"nonce_str", noncestr},
                {"openid", openid},
                {"partner_trade_no", id.ToString()},
                {"re_user_name", username},
                {"spbill_create_ip", spbillcreateip}
            };
            string sign = Sign(x);
            x.Add("sign", sign);

            var (_, xe) = (await WCPay.PostAsync<XElem>("/mmpaymkttransfers/promotion/transfers", x.Dump()));
            string return_code = xe.Child(nameof(return_code));
            if ("SUCCESS" == return_code)
            {
                string result_code = xe.Child(nameof(result_code));
                if ("SUCCESS" != result_code)
                {
                    string err_code_des = xe.Child(nameof(err_code_des));
                    return err_code_des;
                }
            }
            else
            {
                string return_msg = xe.Child(nameof(return_msg));
                return return_msg;
            }
            return null;
        }

        public async Task<(string, string)> PostUnifiedOrderAsync(string trade_no, decimal amount, string openid, string ip, string notifyurl, string descr)
        {
            XElem x = new XElem("xml")
            {
                {"appid", appid},
                {"body", descr},
                {"mch_id", mchid},
                {"nonce_str", noncestr},
                {"notify_url", notifyurl},
                {"openid", openid},
                {"out_trade_no", trade_no},
                {"spbill_create_ip", ip},
                {"total_fee", ((int) (amount * 100)).ToString()},
                {"trade_type", "JSAPI"}
            };
            string sign = Sign(x);
            x.Add("sign", sign);

            var (_, xe) = (await WCPay.PostAsync<XElem>("/pay/unifiedorder", x.Dump()));
            string prepay_id = xe.Child(nameof(prepay_id));
            string err_code = null;
            if (prepay_id == null)
            {
                err_code = xe.Child("err_code");
            }
            return (prepay_id, err_code);
        }

        public bool OnNotified(XElem xe, out string out_trade_no, out decimal total)
        {
            total = 0;
            out_trade_no = null;
            string appid = xe.Child(nameof(appid));
            string mch_id = xe.Child(nameof(mch_id));
            string nonce_str = xe.Child(nameof(nonce_str));

            if (appid != this.appid || mch_id != mchid || nonce_str != noncestr) return false;

            string result_code = xe.Child(nameof(result_code));

            if (result_code != "SUCCESS") return false;

            string sign = xe.Child(nameof(sign));
            xe.Sort();
            if (sign != Sign(xe, "sign")) return false;

            int total_fee = xe.Child(nameof(total_fee)); // in cent
            total = ((decimal) total_fee) / 100;
            out_trade_no = xe.Child(nameof(out_trade_no)); // 商户订单号
            return true;
        }

        public async Task<decimal> PostOrderQueryAsync(string orderno)
        {
            XElem x = new XElem("xml")
            {
                {"appid", appid},
                {"mch_id", mchid},
                {"nonce_str", noncestr},
                {"out_trade_no", orderno}
            };
            string sign = Sign(x);
            x.Add("sign", sign);
            var (_, xe) = (await WCPay.PostAsync<XElem>("/pay/orderquery", x.Dump()));
            sign = xe.Child(nameof(sign));
            xe.Sort();
            if (sign != Sign(xe, "sign")) return 0;

            string return_code = xe.Child(nameof(return_code));
            if (return_code != "SUCCESS") return 0;

            decimal cash_fee = xe.Child(nameof(cash_fee));

            return cash_fee;
        }

        public async Task<string> PostRefundAsync(string orderno, decimal total, decimal cash)
        {
            XElem xo = new XElem("xml")
            {
                {"appid", appid},
                {"mch_id", mchid},
                {"nonce_str", noncestr},
                {"op_user_id", mchid},
                {"out_refund_no", orderno},
                {"out_trade_no", orderno},
                {"refund_fee", ((int) (cash * 100)).ToString()},
                {"total_fee", ((int) (total * 100)).ToString()}
            };
            string sign = Sign(xo);
            xo.Add("sign", sign);

            var (_, xe) = (await WCPay.PostAsync<XElem>("/secapi/pay/refund", xo.Dump()));
            string return_code = xe.Child(nameof(return_code));
            if (return_code != "SUCCESS")
            {
                string return_msg = xe.Child(nameof(return_msg));
                return return_msg;
            }
            string result_code = xe.Child(nameof(result_code));
            if (result_code != "SUCCESS")
            {
                string err_code_des = xe.Child(nameof(err_code_des));
                return err_code_des;
            }
            return null;
        }

        public async Task<string> PostRefundQueryAsync(long orderid)
        {
            XElem xo = new XElem("xml")
            {
                {"appid", appid},
                {"mch_id", mchid},
                {"nonce_str", noncestr},
                {"out_trade_no", orderid.ToString()}
            };
            string sign = Sign(xo);
            xo.Add("sign", sign);
            var (_, xe) = (await WCPay.PostAsync<XElem>("/pay/refundquery", xo.Dump()));

            sign = xe.Child(nameof(sign));
            xe.Sort();
            if (sign != Sign(xe, "sign")) return "返回结果签名错误";

            string return_code = xe.Child(nameof(return_code));
            if (return_code != "SUCCESS")
            {
                string return_msg = xe.Child(nameof(return_msg));
                return return_msg;
            }

            string result_code = xe.Child(nameof(result_code));
            if (result_code != "SUCCESS")
            {
                return "退款订单查询失败";
            }

            string refund_status_0 = xe.Child(nameof(refund_status_0));
            if (refund_status_0 != "SUCCESS")
            {
                return refund_status_0 == "PROCESSING" ? "退款处理中" :
                    refund_status_0 == "REFUNDCLOSE" ? "退款关闭" : "退款异常";
            }

            return null;
        }

        string Sign(XElem xe, string exclude = null)
        {
            StringBuilder sb = new StringBuilder(1024);
            for (int i = 0; i < xe.Count; i++)
            {
                XElem child = xe[i];
                // not include the sign field
                if (exclude != null && child.Tag == exclude) continue;
                if (sb.Length > 0)
                {
                    sb.Append('&');
                }
                sb.Append(child.Tag).Append('=').Append(child.Text);
            }
            sb.Append("&key=").Append(key);
            return TextUtility.MD5(sb.ToString());
        }

        string Sign(JObj jo, string exclude = null)
        {
            StringBuilder sb = new StringBuilder(1024);
            for (int i = 0; i < jo.Count; i++)
            {
                JMbr mbr = jo[i];
                // not include the sign field
                if (exclude != null && mbr.Key == exclude) continue;
                if (sb.Length > 0)
                {
                    sb.Append('&');
                }
                sb.Append(mbr.Key).Append('=').Append((string) mbr);
            }
            sb.Append("&key=").Append(key);
            return TextUtility.MD5(sb.ToString());
        }
    }
}