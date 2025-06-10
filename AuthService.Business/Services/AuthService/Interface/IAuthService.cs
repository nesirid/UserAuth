using AuthService.Business.Dtos;

namespace AuthService.Business.Services.AuthService.Interface
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);

        Task<TokenResponseDto> LoginAsync(LoginDto dto);

        Task<TokenResponseDto> LoginWithRefreshTokenAsync(string refreshToken);

        Task ResetPasswordAsync(string email);

        Task ConfirmPasswordResetAsync(PasswordResetDto dto);

        Task UpdatePasswordAsync(string oldPassword, string newPassword);

        Task CheckUserExistAsync(string email,string phoneNumber);

        Task LogOutAsync(LogoutDto dto);
    }
}