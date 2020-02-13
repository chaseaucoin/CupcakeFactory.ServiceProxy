using System;
using System.Collections.Generic;
using System.Text;

namespace CupcakeFactory.Extensions
{
    internal static class StringExtensions
    {
        public static string ToSHA256Hash(this string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA256.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                //return BitConverter.ToInt64(hashedInputBytes, 0);

                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));

                return hashedInputStringBuilder.ToString();
            }
        }
    }
}
