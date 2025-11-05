using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        string password = "admin123";
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            string hash = Convert.ToBase64String(hashedBytes);
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Hash (Base64): {hash}");
        }
    }
}

