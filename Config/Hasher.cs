using Microsoft.AspNetCore.Identity;
using Serviços.Tabelas;

namespace Serviços.Config
{
    public class Hasher
    {
        private readonly IPasswordHasher<Usuarios> password;

        public Hasher(IPasswordHasher<Usuarios> password)
        {
            this.password = password;
        }

        public string HashPassword(string password)
        {
            return this.password.HashPassword(null, password);
        }

        public bool verificar(string Senha, string Hash)
        {
            var result = this.password.VerifyHashedPassword(null, Hash, Senha);
            return result == PasswordVerificationResult.Success;
        }
    }
}
