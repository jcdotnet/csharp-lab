using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CitiesManager.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthenticationResponse CreateJwtToken(ApplicationUser user)
        {
            // token expiration time
            DateTime expiration = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"]));

            // payload // user's claims
            Claim[] claims = [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // subject
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(type: JwtRegisteredClaimNames.Iat, 
                    value: DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()), // Issued at
                new Claim(type: ClaimTypes.NameIdentifier, value: user.Id.ToString()),
                new Claim(type: ClaimTypes.Name, value: user.Name ?? "Unknown"),
                new Claim(type: ClaimTypes.Email, value: user.Email ?? string.Empty),
            ];

            // secret key
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)    
            );

            // SigningCredential object, with the security key and the HMACSHA256 algorithm
            SigningCredentials signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256
            );

            // create the token object
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: signingCredentials
            );

            // generate the final token as string
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            string token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            return new AuthenticationResponse()
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Expiration = expiration,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiration = DateTime.UtcNow.AddMinutes(
                    Convert.ToInt32(_configuration["RefreshToken:ExpirationMinutes"])
                )
            };
        }

        private static string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
            var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
        {
            var tokenValidationParamenters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
                ),
                ValidateLifetime = false, // token can be expired
            };
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(
                token, tokenValidationParamenters, out SecurityToken securityToken
            );
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, 
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
}
