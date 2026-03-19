using System.Security.Cryptography;
using System.Text;

namespace Serviços.Config
{
    public class HashToken
    {
        public HashToken() { }

        public static string GerarHashToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return string.Empty;
            }

            string Pepper = "fafgqgjeoij1-jtr30o9jrfk90o-ewq2k,fd-02f-2";

            using (var cryptografia = SHA256.Create())
            {
                var cryp = cryptografia.ComputeHash(Encoding.UTF8.GetBytes(refreshToken + Pepper));
                return Convert.ToBase64String(cryp);
            }
        }
    }
}
