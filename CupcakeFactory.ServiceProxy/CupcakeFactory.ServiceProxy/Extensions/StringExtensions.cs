using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CupcakeFactory.Extensions.Hash
{
    public static class StringExtensions
    {
        static SHA256 hasher = SHA256.Create();

        public static byte[] ToSHA256HashBytes(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);            
            var hashedInputBytes = hasher.ComputeHash(bytes);

            return hashedInputBytes;
        }

        public static string ToSHA256ShortHash(this string input)
        {
            var bytes = ToSHA256HashBytes(input);

            return Convert.ToBase64String(bytes);
        }

        public static string ToSHA256HashString(this string input)
        {   
            var hashedInputBytes = ToSHA256HashBytes(input);

                //return BitConverter.ToInt64(hashedInputBytes, 0);

            var hashedInputStringBuilder = new System.Text.StringBuilder(128);

            foreach (var b in hashedInputBytes)
                hashedInputStringBuilder.Append(b.ToString("X2"));

            return hashedInputStringBuilder.ToString();
            
        }
    }
}
