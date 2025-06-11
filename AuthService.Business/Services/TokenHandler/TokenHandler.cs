using AuthService.Business.Dtos;
using AuthService.Business.Services.TokenHandler.Interface;
using AuthService.Core.Entities;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Business.Services.TokenHandler
{
    public class TokenHandler(IConfiguration _configuration) : ITokenHandler
    {
        private const int Argon2DegreeOfParallelism = 4; // Degree of parallelism for Argon2 hashing 4 threads// bu prosesordan asilidi
        private const int Argon2Iterations = 3; // Number of iterations for Argon2 hashing 4 // bu ancaq odensle elaqedar olsa filan eytiyac yoxdu 6-10 
        private const int Argon2MemorySize = 65536; // Memory size in KB for Argon2 hashing 64 MB // mən minimal goydum güvənli 262144–524288
        private const int Argon2HashLength = 32; // Length of the hash in bytes for Argon2 hashing 32 bytes

        public string CreateToken(User user, int expires = 60) // Expires in minutes
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Sid, user.Id.ToString()),
                new(ClaimTypes.Role, user.UserRole.ToString()),
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"] ??
                throw new InvalidOperationException("JWT Signing Key is not configured")));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expires),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreatePasswordResetToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Sid, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"] ??
                throw new InvalidOperationException("JWT Signing Key is not configured")));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GeneratePasswordHash(string input)
        {
            var salt = RandomNumberGenerator.GetBytes(16);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(input))
            {
                Salt = salt,
                DegreeOfParallelism = Argon2DegreeOfParallelism,
                Iterations = Argon2Iterations,
                MemorySize = Argon2MemorySize
            };

            var hash = argon2.GetBytes(Argon2HashLength);

            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public RefreshToken GenerateRefreshToken(string token, int min)
        {
            return new RefreshToken
            {
                Token = GeneratePasswordHash(token),
                Created = DateTime.UtcNow, 
                Expires = DateTime.UtcNow.AddMinutes(min),
            };
        }

        public bool VerifyPasswordHash(string input, string hashedInput)
        {
            try
            {
                var parts = hashedInput.Split(':');
                if (parts.Length != 2) return false;

                var salt = Convert.FromBase64String(parts[0]);
                var originalHash = Convert.FromBase64String(parts[1]);

                var argon2 = new Argon2id(Encoding.UTF8.GetBytes(input))
                {
                    Salt = salt,
                    DegreeOfParallelism = Argon2DegreeOfParallelism,
                    Iterations = Argon2Iterations,
                    MemorySize = Argon2MemorySize
                };

                var newHash = argon2.GetBytes(Argon2HashLength);
                return CryptographicOperations.FixedTimeEquals(originalHash, newHash);
            }
            catch
            {
                return false;
            }
        }
    }
}