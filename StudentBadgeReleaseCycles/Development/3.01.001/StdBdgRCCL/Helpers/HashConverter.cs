using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace StdBdgRCCL.Helpers
{
    static class HashConverter
    {
        public static Int64 Hash(string stringToHash)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                return BitConverter.ToInt64(sha1.ComputeHash(Encoding.UTF8.GetBytes(stringToHash)), 0);
            }

        }
    }
}
