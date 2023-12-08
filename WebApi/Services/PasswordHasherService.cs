using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Services
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }

    public class PasswordHasherService : IPasswordHasherService
    {
        private const int IterationCount = 10000; // Adjust based on your security needs

        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password using PBKDF2
            string hashedPassword = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: IterationCount,
                    numBytesRequested: 256 / 8
                )
            );

            // Combine salt and hashed password for storage
            return $"{Convert.ToBase64String(salt)}|{hashedPassword}";
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            // Extract salt and hashed password from the stored value
            string[] parts = hashedPassword.Split('|');
            byte[] salt = Convert.FromBase64String(parts[0]);
            string storedHashedPassword = parts[1];

            // Hash the provided password with the extracted salt
            string hashedProvidedPassword = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: providedPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: IterationCount,
                    numBytesRequested: 256 / 8
                )
            );

            // Compare the stored hashed password with the newly hashed provided password
            return storedHashedPassword.Equals(hashedProvidedPassword);
        }
    }
}
