using AuthService.Business.Dtos;
using AuthService.Core.Entities;

namespace AuthService.Business.Services.TokenHandler.Interface
{
    public interface ITokenHandler
    {
        string CreateToken(User user, int expires = 60);
        string CreatePasswordResetToken(User user);
        string GeneratePasswordHash(string input);
        RefreshToken GenerateRefreshToken(string token, int min);
        bool VerifyPasswordHash(string input, string hashedInput);
        Guid GetUserIdFromToken(string token);
    }
}
