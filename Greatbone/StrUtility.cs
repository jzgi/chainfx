using System;
using System.Security.Cryptography;
using System.Text;

namespace Greatbone
{
    public static class StrUtility
    {
        // hexidecimal numbers
        static readonly char[] HEX = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public static string ToHex(ulong v)
        {
            char[] buf = new char[16];
            for (int i = 0; i < 16; i++)
            {
                buf[i] = HEX[(v >> (i * 4)) & 0x0fL];
            }
            return new string(buf);
        }

        // days of week
        static readonly string[] DOW = {"Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"};

        // sexagesimal numbers
        static readonly string[] SEX =
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59"
        };

        // months
        static readonly string[] MON =
        {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        // HTTP date format
        public static string FormatDate(DateTime v)
        {
            v = v.ToUniversalTime();

            StringBuilder sb = new StringBuilder();
            int yr = v.Year;

            if (yr < 1000) sb.Append('0');
            if (yr < 100) sb.Append('0');
            if (yr < 10) sb.Append('0');
            sb.Append(v.Year);
            sb.Append('-');
            sb.Append(SEX[v.Month - 1]);
            sb.Append('-');
            sb.Append(SEX[v.Day]);

            sb.Append(SEX[v.Hour]);
            sb.Append(':');
            sb.Append(SEX[v.Minute]);
            sb.Append(':');
            sb.Append(SEX[v.Second]);

            return sb.ToString();
        }

        // HTTP date format
        public static string FormatUtcDate(DateTime v)
        {
            v = v.ToUniversalTime();

            StringBuilder gmt = new StringBuilder();
            gmt.Append(DOW[(int) v.DayOfWeek]);
            gmt.Append(", ");

            gmt.Append(SEX[v.Day]);
            gmt.Append(' ');
            gmt.Append(MON[v.Month - 1]);
            gmt.Append(' ');
            gmt.Append(v.Year);
            gmt.Append(' ');

            gmt.Append(SEX[v.Hour]);
            gmt.Append(':');
            gmt.Append(SEX[v.Minute]);
            gmt.Append(':');
            gmt.Append(SEX[v.Second]);

            gmt.Append(" GMT");

            return gmt.ToString();
        }

        public static bool TryParseDate(string str, out DateTime v)
        {
            int year = ParseInt(str, 0, 4, 1000);
            int month = ParseInt(str, 5, 2, 10);
            int day = ParseInt(str, 8, 2, 10);
            int len = str.Length;

            int hour = 0, minute = 0, second = 0; // optional time part
            if (len >= 19)
            {
                hour = ParseInt(str, 11, 2, 10);
                minute = ParseInt(str, 14, 2, 10);
                second = ParseInt(str, 17, 2, 10);
            }
            try
            {
                v = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
                return true;
            }
            catch
            {
                v = default;
                return false;
            }
        }

        public static bool TryParseUtcDate(string utcstr, out DateTime v)
        {
            int day = ParseInt(utcstr, 5, 2, 10);
            int month = ParseMonth(utcstr, 8);
            int year = ParseInt(utcstr, 12, 4, 1000);
            int hour = ParseInt(utcstr, 17, 2, 10);
            int minute = ParseInt(utcstr, 20, 2, 10);
            int second = ParseInt(utcstr, 23, 2, 10);
            try
            {
                v = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
                return true;
            }
            catch
            {
                v = default;
                return false;
            }
        }

        static int ParseInt(string str, int start, int count, int @base)
        {
            int num = 0;
            for (int i = 0; i < count; i++, @base /= 10)
            {
                char c = str[start + i];
                int digit = c - '0';
                if (digit < 0 || digit > 9) digit = 0;
                num += digit * @base;
            }
            return num;
        }

        static int ParseMonth(string str, int start)
        {
            char a = str[start], b = str[start + 1], c = str[start + 2];
            for (int i = 0; i < MON.Length; i++)
            {
                string m = MON[i];
                if (a == m[0] && b == m[1] && c == m[2])
                {
                    return i + 1;
                }
            }
            return 0;
        }

        //
        // DIGEST
        //

        public static string MD5(string src)
        {
            if (src == null) return null;

            byte[] raw = Encoding.UTF8.GetBytes(src);

            // digest and transform
            using (MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hash = md5.ComputeHash(raw);
                StringBuilder str = new StringBuilder(32);
                for (int i = 0; i < 16; i++)
                {
                    byte b = hash[i];
                    str.Append(HEX[b >> 4]);
                    str.Append(HEX[b & 0x0f]);
                }
                return str.ToString();
            }
        }

        public static string SHA1(string src)
        {
            if (src == null) return null;

            byte[] raw = Encoding.UTF8.GetBytes(src);

            // digest and transform
            using (SHA1 sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(raw);
                StringBuilder str = new StringBuilder(32);
                for (int i = 0; i < 16; i++)
                {
                    byte b = hash[i];
                    str.Append(HEX[b >> 4]);
                    str.Append(HEX[b & 0x0f]);
                }
                return str.ToString();
            }
        }

        /// <summary>
        /// Returns the central 8 bytes of the hash result of the input string.
        /// </summary>
        public static bool EqualsCredential(this string credential, string id, string pass)
        {
            if (credential == null || credential.Length != 32) return false;

            // convert to bytea, assume ascii 
            int idlen = id.Length;
            int passlen = pass.Length;
            int len = idlen + passlen + 1;
            byte[] raw = new byte[len];
            int p = 0;
            for (int i = 0; i < idlen; i++)
            {
                raw[p++] = (byte) id[i];
            }
            raw[p++] = (byte) ':';
            for (int i = 0; i < passlen; i++)
            {
                raw[p++] = (byte) pass[i];
            }

            // digest and transform
            using (MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hash = md5.ComputeHash(raw);
                for (int i = 0; i < 16; i++)
                {
                    byte b = hash[i];
                    if (credential[i * 2] != HEX[b >> 4] || credential[i * 2 + 1] != HEX[b & 0x0f])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string ToHex(string v)
        {
            int vlen = v.Length;
            char[] buf = new char[vlen * 4];
            for (int i = 0; i < vlen; i++)
            {
                char c = v[i];
                buf[i * 4 + 0] = HEX[(c & 0xf000) >> 12];
                buf[i * 4 + 1] = HEX[(c & 0x0f00) >> 8];
                buf[i * 4 + 2] = HEX[(c & 0x00f0) >> 4];
                buf[i * 4 + 3] = HEX[c & 0x000f];
            }
            return new string(buf);
        }

        public static string FromHex(string v)
        {
            int vlen = v.Length;
            char[] buf = new char[vlen / 4];
            int i = 0;
            while (i < vlen)
            {
                int m = i / 4;
                char c = (char) ((Dv(v[i++]) << 12) + (Dv(v[i++]) << 8) + (Dv(v[i++]) << 4) + Dv(v[i++]));
                buf[m] = c;
            }
            return new string(buf);
        }

        static int Dv(char hex)
        {
            int num = hex - 'a';
            if (num >= 0 && num <= 5)
            {
                return num + 10;
            }
            num = hex - '0';
            if (num >= 0 && num <= 9)
            {
                return num;
            }
            return -1;
        }

        // UTF-8 encoding
        public static ArraySegment<byte> ToUtf8(string text)
        {
            int len = text.Length;
            byte[] buf = new byte[len * 3]; // sufficient capacity
            int p = 0; // current position in the buffer
            for (int i = 0; i < len; i++)
            {
                char c = text[i];
                // UTF-8 encoding (without surrogate support)
                if (c < 0x80)
                {
                    // have at most seven bits
                    buf[p++] = ((byte) c);
                }
                else if (c < 0x800)
                {
                    // 2 text, 11 bits
                    buf[p++] = (byte) (0xc0 | (c >> 6));
                    buf[p++] = (byte) (0x80 | (c & 0x3f));
                }
                else
                {
                    // 3 text, 16 bits
                    buf[p++] = (byte) (0xe0 | (c >> 12));
                    buf[p++] = (byte) (0x80 | (c >> 6) & 0x3f);
                    buf[p++] = (byte) (0x80 | (c & 0x3f));
                }
            }
            return new ArraySegment<byte>(buf, 0, p);
        }


        //
        // CONVERTION
        //

        public static bool ToBool(this string str)
        {
            return str == "true" || str == "1";
        }

        public static short ToShort(this string str)
        {
            return (short) str.ToInt(0, str.Length);
        }

        public static int ToInt(this string str, int start, int end)
        {
            int sum = 0;
            for (int i = start; i < end; i++)
            {
                char c = str[i];
                int n = c - '0';
                if (n >= 0 && n <= 9)
                {
                    sum = sum * 10 + n;
                }
            }
            return sum;
        }

        public static int ToInt(this string str)
        {
            return str.ToInt(0, str.Length);
        }

        public static long ToLong(this string str, int start, int end)
        {
            long sum = 0;
            for (int i = start; i < end; i++)
            {
                char c = str[i];
                int n = c - '0';
                if (n > 0 && n <= 10)
                {
                    sum = sum * 10 + n;
                }
            }
            return sum;
        }

        public static long ToLong(this string str)
        {
            return str.ToLong(0, str.Length);
        }

        public static DateTime ToDateTime(this string str)
        {
            TryParseDate(str, out var v);
            return v;
        }

        public static (int, int) To2Ints(this string str, char sep = '-')
        {
            int len = str.Length;
            int p = 0;
            while (p < len && str[p] != sep) p++;

            int a = str.ToInt(0, p);
            int b = str.ToInt(p + 1, len);
            return (a, b);
        }

        public static (int, string) ToIntString(this string str, char sep = '-')
        {
            int p = 0;
            int len = str.Length;
            while (p < len && str[p] != sep)
            {
                p++;
            }
            int a = str.ToInt(0, p);
            string b = str.Substring(p + 1, len - (p + 1));
            return (a, b);
        }

        static bool TryParseTill(this string str, ref int pos, out string v, char sep)
        {
            int len = str.Length;
            if (pos >= len)
            {
                v = null;
                return false;
            }
            int p = pos;
            while (p < len && str[p] != sep) p++;
            v = p == len ? str : str.Substring(pos, p);
            pos = p;
            return true;
        }

        public static bool TryParseTill(this string str, ref int pos, out int v, char sep)
        {
            int len = str.Length;
            if (pos >= len)
            {
                v = 0;
                return false;
            }
            int sum = 0;
            int p = pos;
            while (p < len && str[p] != sep)
            {
                char c = str[p];
                int n = c - '0';
                if (n >= 0 && n <= 9)
                {
                    sum = sum * 10 + n;
                }
                p++;
            }
            v = sum;
            pos = p;
            return true;
        }


        public static (string, string) ToDual(this string str, char sep = '-')
        {
            if (str == null)
            {
                return (null, null);
            }
            int len = str.Length;
            int p = 0;
            while (p < len && str[p] != sep) p++;
            if (p == len)
            {
                return (str, null);
            }
            string a = str.Substring(0, p);
            string b = str.Substring(p + 1, len - (p + 1));
            return (a, b);
        }

        public static (string, string, string) ToTriple(this string str, char sep = ' ')
        {
            int len = str.Length;

            int p0 = 0, p = 0;
            while (p < len && str[p] != sep) p++;
            if (p == len)
            {
                return (str, null, null);
            }
            string a = str.Substring(p0, p - p0);

            p0 = ++p;
            while (p < len && str[p] != sep) p++;
            string b = str.Substring(p0, p - p0);
            if (p == len)
            {
                return (a, b, null);
            }

            p0 = ++p;
            string c = str.Substring(p0, len - p0);
            return (a, b, c);
        }
    }
}