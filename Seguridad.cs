using System;
using System.Security.Cryptography;
using System.Text;

namespace Movelabu.Clases
{
    public static class Seguridad
    {
        // Convierte "admin123" en un código de 64 caracteres (SHA-256).
        // Es el mismo resultado que da SHA2('admin123', 256) en MySQL.
        public static string Encriptar(string texto)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));   // cada byte en hexadecimal
                return sb.ToString();
            }
        }
    }
}