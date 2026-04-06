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
        private readonly string _SecretKeyConfirm;
        private readonly string _issuerConfirm;
        private readonly string _audienceConfirm;

        public GenerateToken(IConfiguration configuration)
        {
            _SecretKey = configuration["JwtSettings:Secret"];
            _issuer = configuration["JwtSettings:Issuer"];
            _audience = configuration["JwtSettings:Audience"];
            _SecretKeyConfirm = configuration["JwtSettings:SecretConfirm"];
            _issuerConfirm = configuration["JwtSettings:IssuerConfirm"];
            _audienceConfirm = configuration["JwtSettings:AudienceConfirm"];
        }
        public string GerarToken(string Nome, string Email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_SecretKey));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, Nome),
                new Claim(ClaimTypes.Email, Email),
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

        public string GerarTokenConfirmacao(string Nome, string Email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_SecretKeyConfirm));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, Nome),
                new Claim(ClaimTypes.Email, Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuerConfirm,
                audience: _audienceConfirm,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credential

            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
