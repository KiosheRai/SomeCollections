using System.Security.Cryptography;
using System.Text;

namespace SomeCollections.Models
{
    public static class HashGenerator
    {
        public static string Generate(string str)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(str));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
