using System;
using System.Text;

namespace WinFormDemo.CommUtil 
{
    class PkgUtil
    {
        public static byte[] BytesAdd(byte[] added, byte[] add)
        {
            byte[] dest = new byte[added.Length + add.Length];
            Buffer.BlockCopy(added, 0, dest, 0, added.Length);
            Buffer.BlockCopy(add, 0, dest, added.Length, add.Length);
            return dest;
        }

        public static byte[] reverseOrder(byte[] src)
        {
            byte[] dest = new byte[src.Length];
            for (int i = 0; i < src.Length; i++)
            {
                dest[i] = src[src.Length - 1 - i];
            }
            return dest;
        }
        public static String toHexString(byte[] src)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < src.Length; i++)
            {
                sb.Append(String.Format("{0:X2}", src[i]));
                sb.Append(" ");
            }
            return sb.ToString();
        }
        public static int bytesIndexOf(byte[] s, byte[] pattern)
        {
            int slen = s.Length;
            int plen = pattern.Length;
            for (int i = 0, j; i <= slen - plen; i++)
            {
                for (j = 0; j < plen; j++)
                {
                    if (s[i + j] != pattern[j])
                    {
                        break;
                    }
                }
                if (j == plen)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
