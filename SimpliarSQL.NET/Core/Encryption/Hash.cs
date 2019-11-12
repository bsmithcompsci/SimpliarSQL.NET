using System;
using System.Text;
using System.Security.Cryptography;

namespace SimpliarSQL.NET.Core.Encryption
{
    public class Hash
    {
        public static string Sha256(string raw)
        {
            using (SHA256 hash = SHA256.Create())
            {
                byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(raw));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    builder.Append(bytes[i].ToString("x2"));

                // Memory clean...
                hash.Dispose();

                return builder.ToString();
            }
        }

        public static string GenerateSeed(uint range = 32)
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[range];
                rng.GetBytes(tokenData);

                // Memory clean...
                rng.Dispose();
                return Convert.ToBase64String(tokenData);
            }
        }
    }
}
