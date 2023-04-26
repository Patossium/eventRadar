using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;



namespace eventRadar.Auth
{
    public interface IJwtTokenService
    {
        string CreateAccessToken(string username, string userId, IEnumerable<string> userRoles);
    }
    public class JwtTokenService : IJwtTokenService
    {
        private readonly SymmetricSecurityKey _authSigningKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtTokenService(IConfiguration configuration)
        {
            _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            _issuer = configuration["JWT:ValidIssuer"];
            _audience = configuration["JWT:ValidAudience"];
        }
        public string CreateAccessToken(string username, string userId, IEnumerable<string> userRoles)
        {
            var authClaims = new List<Claim>
            {
                new(type: ClaimTypes.Name, value: username),
                new(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
                new(type: JwtRegisteredClaimNames.Sub, value: userId.ToString()),
            };

            authClaims.AddRange(collection: userRoles.Select(userRole => new Claim(type: ClaimTypes.Role, value: userRole)));
            var accessSecurityToken = new JwtSecurityToken
            (
                issuer: _issuer,
                audience: _audience,
                expires: DateTime.UtcNow.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(_authSigningKey, algorithm: SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(accessSecurityToken);
        }
    }
}
