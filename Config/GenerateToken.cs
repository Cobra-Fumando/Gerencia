using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Serviços.Config
{
    public class GenerateToken
    {
        private readonly string _SecretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public GenerateToken(IConfiguration configuration)
        {
            _SecretKey = configuration["JwtSettings:Secret"];
            _issuer = configuration["JwtSettings:Issuer"];
            _audience = configuration["JwtSettings:Audience"];
        }
        public string GerarToken(string Nome)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_SecretKey));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, Nome),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credential

            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
