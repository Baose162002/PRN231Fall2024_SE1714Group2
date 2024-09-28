using System;
using System.Security.Cryptography;

namespace EventFlowerExchange.Utilities
{
    class GenerateJwtSecret
    {
        static void Main(string[] args)
        {
            var key = GenerateRandomKey(32); // 256 bits
            Console.WriteLine($"Your JWT Secret Key: {key}");
        }

        static string GenerateRandomKey(int bytes)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var buffer = new byte[bytes];
                rng.GetBytes(buffer);
                return Convert.ToBase64String(buffer);
            }
        }
    }
}