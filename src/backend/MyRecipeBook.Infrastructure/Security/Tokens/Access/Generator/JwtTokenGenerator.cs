using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.Infrastructure.Security.Tokens.Access.Generator
{
    public class JwtTokenGenerator : IAccessTokenGenerator
    {
        private readonly string _signingKey;
        private readonly uint _expirationTimeMinutes;

        public JwtTokenGenerator(string signingKey, uint expirationTimeMinutes)
        {
            _signingKey = signingKey;
            _expirationTimeMinutes = expirationTimeMinutes;
        }

        public string Generate(Guid userIdentifier)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Sid, userIdentifier.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_expirationTimeMinutes),
                SigningCredentials = new SigningCredentials(SecurityeKey(), SecurityAlgorithms.HmacSha256Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }

        private SymmetricSecurityKey SecurityeKey()
        {
            var bytes = Encoding.UTF8.GetBytes(_signingKey);

            return new SymmetricSecurityKey(bytes);
        }
    }
}
