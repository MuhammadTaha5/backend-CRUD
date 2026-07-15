using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyFirstAPI.Models;
namespace MyFirstAPI.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }
        /// <summary>
        /// this generates the jwt token used to access resource and protected endpoints
        /// </summary>
        /// <param name="user">The user which successfully validated credentials</param>
        /// <param name="roles">Roles to add in jwt token</param>
        /// <returns></returns>
        public string GenerateAccessToken(AppUser user, IList<string> roles)
        {
            List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // unique token id
        };

            // add roles as claims
            foreach (string role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_config["JwtSettings:AccessTokenExpiryMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}