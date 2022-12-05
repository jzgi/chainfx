using System;
using System.Text;

namespace ChainFx
{
    public static class CryptoUtility
    {
        public static void Encrypt(byte[] buf, int len, uint[] key)
        {
            uint k0 = key[0], k1 = key[1], k2 = key[2], k3 = key[3]; // cache key
            const uint delta = 0x9E3779B9; // a key schedule constant
            unsafe
            {
                fixed (byte* p = buf)
                {
                    uint* pa = (uint*) p, pb = (uint*) p;
                    for (int seg = 0; seg < len - 8; seg += 8) // NOTE: up to 7 bytes ignored 
                    {
                        uint v0 = *pa++, v1 = *pa++, sum = 0; // set up
                        for (int i = 0; i < 32; i++)
                        {
                            sum += delta;
                            v0 += ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                            v1 += ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
                        }

                        *pb++ = v0;
                        *pb++ = v1;
                    }
                }
            }
        }

        public static void Decrypt(byte[] buf, int len, uint[] key)
        {
            uint k0 = key[0], k1 = key[1], k2 = key[2], k3 = key[3]; // cache key
            const uint delta = 0x9E3779B9; // a key schedule constant
            unsafe
            {
                fixed (byte* p = buf)
                {
                    uint* pa = (uint*) p, pb = (uint*) p;
                    for (int seg = 0; seg < len - 8; seg += 8)
                    {
                        uint v0 = *pa++, v1 = *pa++, sum = 0xC6EF3720; // set up
                        for (int i = 0; i < 32; i++)
                        {
                            v1 -= ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
                            v0 -= ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                            sum -= delta;
                        }

                        *pb++ = v0;
                        *pb++ = v1;
                    }
                }
            }
        }

        public static string ComputeVCode(string tel)
        {
            string str = tel + ":" + Application.Secret;

            // digest and transform
            long v = 0;
            Digest(str, ref v);
            var sb = new StringBuilder();
            sb.Append(Math.Abs((v >> 48) % 10)).Append(Math.Abs((v >> 32) % 10)).Append(Math.Abs((v >> 16) % 10)).Append(Math.Abs((v) % 10));

            return sb.ToString();
        }


        static readonly char[] HEX = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'};

        static readonly char[] HEXU = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public static uint[] HexToKey(string v)
        {
            int vlen = v.Length;
            if (vlen != 32)
            {
                throw new ArgumentException("crypto key must have 32 hex characters");
            }

            var buf = new uint[4];
            int i = 0;
            while (i < vlen)
            {
                int m = i / 8;
                uint c = ((Dv(v[i++]) << 28) + (Dv(v[i++]) << 24) + (Dv(v[i++]) << 20) + (Dv(v[i++]) << 16) + (Dv(v[i++]) << 12) + (Dv(v[i++]) << 8) + (Dv(v[i++]) << 4) + Dv(v[i++]));
                buf[m] = c;
            }

            return buf;
        }

        public static string ToHex(long v)
        {
            const int LEN = 16;
            var buf = new char[LEN];
            for (int i = 0; i < LEN; i++)
            {
                buf[LEN - i - 1] = HEX[(v >> (i * 4)) & 0x0fL];
            }
            return new string(buf);
        }

        public static string ToHex(int v)
        {
            const int LEN = 8;
            var buf = new char[LEN];
            for (int i = 0; i < LEN; i++)
            {
                buf[LEN - i - 1] = HEX[(v >> (i * 4)) & 0x0f];
            }
            return new string(buf);
        }

        public static string ToHex(short v)
        {
            const int LEN = 4;
            var buf = new char[LEN];
            for (int i = 0; i < LEN; i++)
            {
                buf[LEN - i - 1] = HEX[(v >> (i * 4)) & 0x0f];
            }
            return new string(buf);
        }


        public static string BytesToHex(byte[] bytes, int count)
        {
            var buf = new char[count * 2];
            for (int i = 0; i < count; i++)
            {
                int b = bytes[i];
                buf[i * 2] = HEX[(b & 0xf0) >> 4];
                buf[i * 2 + 1] = HEX[b & 0x0f];
            }

            return new string(buf);
        }

        public static byte[] HexToBytes(string str)
        {
            int strlen = str.Length;
            var buf = new byte[strlen / 2];
            for (int i = 0; i < buf.Length; i++)
            {
                byte b = (byte) ((Dv(str[i * 2]) << 4) | Dv(str[i * 2 + 1]));
                buf[i] = b;
            }

            return buf;
        }

        public static string StringToHex(string str)
        {
            var len = str.Length;
            var buf = new char[len * 4];
            for (int i = 0; i < len; i++)
            {
                char c = str[i];
                buf[i * 4] = HEX[(c & 0x0f)];
                buf[i * 4 + 1] = HEX[(c & 0x00f0) >> 4];
                buf[i * 4 + 2] = HEX[(c & 0x0f00) >> 8];
                buf[i * 4 + 3] = HEX[(c & 0xf000) >> 12];
            }

            return new string(buf);
        }

        public static string HexToString(string str)
        {
            int strlen = str.Length;
            var buf = new char[strlen / 4];
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (char) ((Dv(str[i * 4]) << 4) | Dv(str[i * 4 + 1]) | Dv(str[i * 4 + 2]) | Dv(str[i * 4 + 4]));
            }

            return new string(buf);
        }


        static uint Dv(char hex)
        {
            int num = hex - 'a';
            if (num >= 0 && num <= 5)
            {
                return (uint) (num + 10);
            }
            num = hex - '0';
            if (num >= 0 && num <= 9)
            {
                return (uint) num;
            }
            return 0;
        }

        public static string MD5(string src, bool uppercase = false)
        {
            if (src == null) return null;

            var arr = uppercase ? HEXU : HEX;

            var raw = Encoding.UTF8.GetBytes(src);

            // digest and transform
            using var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(raw);
            var str = new StringBuilder(32);
            for (int i = 0; i < 16; i++)
            {
                var b = hash[i];
                str.Append(arr[b >> 4]);
                str.Append(arr[b & 0x0f]);
            }

            return str.ToString();
        }

        public static string MD5(byte[] src, int len, bool uppercase = false)
        {
            if (src == null) return null;

            var harr = uppercase ? HEXU : HEX;

            // digest and transform
            using var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(src, 0, len);
            var str = new StringBuilder(32);
            for (int i = 0; i < 16; i++)
            {
                var b = hash[i];
                str.Append(harr[b >> 4]);
                str.Append(harr[b & 0x0f]);
            }

            return str.ToString();
        }


        public static string SHA1(string src)
        {
            if (src == null) return null;

            var raw = Encoding.UTF8.GetBytes(src);

            // digest and transform
            using var sha1 = System.Security.Cryptography.SHA1.Create();
            var hash = sha1.ComputeHash(raw);
            var str = new StringBuilder(32);
            for (int i = 0; i < 20; i++)
            {
                byte b = hash[i];
                str.Append(HEX[b >> 4]);
                str.Append(HEX[b & 0x0f]);
            }

            return str.ToString();
        }


        public static void Digest(string v, ref long hash)
        {
            if (v != null)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    var c = v[i];
                    DigestByte((byte) c, ref hash);
                    DigestByte((byte) (c >> 8), ref hash);
                }
            }
        }


        public static void Digest(long v, ref long hash)
        {
            DigestByte((byte) v, ref hash);
            DigestByte((byte) (v >> 8), ref hash);
            DigestByte((byte) (v >> 16), ref hash);
            DigestByte((byte) (v >> 24), ref hash);
            DigestByte((byte) (v >> 32), ref hash);
            DigestByte((byte) (v >> 40), ref hash);
            DigestByte((byte) (v >> 48), ref hash);
            DigestByte((byte) (v >> 56), ref hash);
        }

        static void DigestByte(byte b, ref long hash)
        {
            var cs = hash;

            cs ^= b << ((b & 0b00000111) * 8);
            unchecked
            {
                cs *= ((b & 0b00011000) >> 3) switch {0 => 7, 1 => 11, 2 => 13, _ => 17};
            }
            cs ^= ~b << (((b & 0b11100000) >> 5) * 8);

            hash = cs;
        }


        public static void Test()
        {
            var bs = HexToBytes("ebce206103e439514f3c4748c683274e");
            string sssdf = BytesToHex(bs, bs.Length);

            uint[] key = {0x34a3, 0x34a3, 0x34a3, 0x34a3};

            var c = new TextBuilder(true, 1024);

            c.Add("we are a plain tex tstring characters, okay!");

            Encrypt(c.Buffer, c.Count, key);

            var s1 = Encoding.UTF8.GetString(c.Buffer, 0, c.Count);

            Decrypt(c.Buffer, c.Count, key);

            var s2 = Encoding.UTF8.GetString(c.Buffer, 0, c.Count);
        }
    }
}