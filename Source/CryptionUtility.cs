using System;
using System.Text;

namespace CloudUn
{
    public class CryptionUtility
    {
        public static void Encrypt(byte[] buf, int len, uint[] key)
        {
            uint k0 = key[0], k1 = key[1], k2 = key[2], k3 = key[3]; // cache key
            const uint delta = 0x9E3779B9; // a key schedule constant
            unsafe
            {
                fixed (byte* p = &buf[0])
                {
                    uint* pa = (uint*) p, pb = (uint*) p;
                    for (int seg = 0; seg < len; seg += 8)
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
                fixed (byte* p = &buf[0])
                {
                    uint* pa = (uint*) p, pb = (uint*) p;
                    for (int seg = 0; seg < len; seg += 8)
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

        public static uint[] HexToKey(string v)
        {
            int vlen = v.Length;
            if (vlen != 32)
            {
                throw new ArgumentException("crypto key must have 32 hex characters");
            }

            uint[] buf = new uint[4];
            int i = 0;
            while (i < vlen)
            {
                int m = i / 8;
                uint c = ((Dv(v[i++]) << 28) + (Dv(v[i++]) << 24) + (Dv(v[i++]) << 20) + (Dv(v[i++]) << 16) + (Dv(v[i++]) << 12) + (Dv(v[i++]) << 8) + (Dv(v[i++]) << 4) + Dv(v[i++]));
                buf[m] = c;
            }
            return buf;
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


        public static void Test()
        {
            var bs  =TextUtility.HexToBytes("ebce206103e439514f3c4748c683274e");
            string sssdf =TextUtility.BytesToHex(bs, bs.Length);
            
            uint[] key = {0x34a3, 0x34a3, 0x34a3, 0x34a3};

            var c = new TextContent(1024);

            c.Add("we are a plain tex tstring characters, okay!");

            Encrypt(c.Buffer, c.Count, key);

            var s1 = Encoding.UTF8.GetString(c.Buffer, 0, c.Count);

            Decrypt(c.Buffer, c.Count, key);

            var s2 = Encoding.UTF8.GetString(c.Buffer, 0, c.Count);
        }
    }
}