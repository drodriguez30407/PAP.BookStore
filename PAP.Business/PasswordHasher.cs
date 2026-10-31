using System;
using System.Globalization;
using System.Security.Cryptography;

namespace PAP.Business
{
    internal static class PasswordHasher
    {
        internal const int MinLength = 15;
        internal const int MaxLength = 128;
        private const int Iterations = 600000;

        internal static string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < MinLength
                || password.Length > MaxLength)
                throw new ArgumentException("La contraseña debe tener entre 15 y 128 caracteres.");

            byte[] salt = new byte[16];
            using (var random = RandomNumberGenerator.Create()) random.GetBytes(salt);
            using (var derive = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = derive.GetBytes(32);
                return "PBKDF2-SHA256$" + Iterations.ToString(CultureInfo.InvariantCulture)
                    + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
            }
        }
    }
}
