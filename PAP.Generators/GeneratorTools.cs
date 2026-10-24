using System;
using System.Globalization;
using System.Security.Cryptography;

namespace PAP.Generators
{
    internal static class GeneratorTools
    {
        internal static string Key() { return Guid.NewGuid().ToString("N").Substring(0, 8); }

        internal static void RequireParents(int count, string name)
        {
            if (count == 0) throw new GenerationValidationException("Primero genere datos de " + name + ".");
        }

        internal static int PairCount(int requested, int left, int right)
        {
            RequireParents(left, "la primera tabla relacionada");
            RequireParents(right, "la segunda tabla relacionada");
            return (int)Math.Min(requested, (long)left * right);
        }

        internal static string PasswordHash()
        {
            using (var sha = SHA256.Create())
                return Convert.ToBase64String(sha.ComputeHash(Guid.NewGuid().ToByteArray()));
        }

        internal static string IsbnPrefix()
        {
            var bytes = new byte[4];
            using (var random = RandomNumberGenerator.Create()) random.GetBytes(bytes);
            return "978" + (BitConverter.ToUInt32(bytes, 0) % 1000000).ToString("D6", CultureInfo.InvariantCulture);
        }

        internal static string Isbn(string prefix, int index)
        {
            string digits = prefix + index.ToString("D3", CultureInfo.InvariantCulture);
            int sum = 0;
            for (int i = 0; i < digits.Length; i++) sum += (digits[i] - '0') * (i % 2 == 0 ? 1 : 3);
            return digits + ((10 - sum % 10) % 10).ToString(CultureInfo.InvariantCulture);
        }
    }
}
