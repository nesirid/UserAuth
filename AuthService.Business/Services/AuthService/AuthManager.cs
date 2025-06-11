using AuthService.Business.Dtos;
using AuthService.Business.Services.AuthService.Interface;
using AuthService.Core.Entities;
using AuthService.DAL.Contexts;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using AuthService.Business.Exceptions.UserException;
using SharedLibrary.Exceptions;
using AuthService.Business.Services.TokenHandler.Interface;
using SharedLibrary.HelperServices.Current.Interface;

namespace AuthService.Business.Services.AuthService
{
    public class AuthManager : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenHandler _tokenHandler;
        private readonly IConfiguration _configuration;
        private readonly ICurrentUser _currentUser;

        public AuthManager(
               AppDbContext context,
               ITokenHandler tokenHandler,
               IConfiguration configuration,
               ICurrentUser currentUser
                          )
        {
            _context = context;
            _tokenHandler = tokenHandler;
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            dto.MainPhoneNumber = dto.MainPhoneNumber.Trim();
            dto.Email = dto.Email.Trim().ToLower();

            if (dto.Password != dto.ConfirmPassword)
                throw new WrongPasswordException();

            bool emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                throw new UserExistException();

            bool phoneExists = await _context.Users.AnyAsync(u => u.MainPhoneNumber == dto.MainPhoneNumber);
            if (phoneExists)
                throw new UserExistException();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                UserName = dto.UserName.Trim(),
                JobPosition = dto.JobPosition.Trim(),
                MainPhoneNumber = dto.MainPhoneNumber,
                RegistrationDate = DateTime.UtcNow,
                Password = _tokenHandler.GeneratePasswordHash(dto.Password),
                UserRole = UserRole.SimpleUser,
                Status = UserStatus.Active
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }


        public async Task BlockUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new NotFoundException<User>();

            if ((UserRole)_currentUser.UserRole != UserRole.SuperAdmin)
                throw new UnauthorizedAccessException("Only super administrators can block users");

            user.Status = UserStatus.Blocked;
            user.RefreshToken = null;
            user.RefreshTokenExpireDate = null;

            await _context.SaveChangesAsync();
        }

        public async Task UnBlockUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new NotFoundException<User>();

            if ((UserRole)_currentUser.UserRole != UserRole.SuperAdmin)
                throw new UnauthorizedAccessException("Only super administrators can block users");

            user.Status = UserStatus.Active;
            user.FailedLoginAttempts = 0;
            user.RefreshToken = null;
            user.RefreshTokenExpireDate = null;

            await _context.SaveChangesAsync();
        }

        public async Task CheckUserExistAsync(UserCheckDto dto)
        {
            if (await _context.Users.AsNoTracking().AnyAsync(u => u.Email == dto.Email.ToLower()))
                throw new ExistEmailException();

            if (await _context.Users.AsNoTracking().AnyAsync(u => u.MainPhoneNumber == dto.PhoneNumber))
                throw new ExistPhoneNumberException();
        }

        //public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        //{
        //    var normalizedInput = dto.UserNameOrEmail.ToLower();

        //    var user = await _context.Users
        //        .FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedInput || x.UserName.ToLower() == normalizedInput)
        //        ?? throw new LoginFailedException();

        //    if (user.Status != UserStatus.Active)
        //        throw new InvalidUserStatusException($"User status is not active: {user.Status}");

        //    if (!_tokenHandler.VerifyPasswordHash(dto.Password, user.Password))
        //        throw new LoginFailedException();

        //    return await GenerateTokenResponse(user);
        //}

        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            var normalizedInput = dto.UserNameOrEmail.ToLower().Trim();

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedInput || x.UserName.ToLower() == normalizedInput)
                ?? throw new LoginFailedException("Login ve ya parol səfdir");

            if (user.Status == UserStatus.Blocked)
            {
                throw new Exception("İstifadəci bloklanıb");
            }

            if (user.Status != UserStatus.Active)
            {
                throw new InvalidUserStatusException($"İstifadəci Aktiv deyil {user.Status}");
            }

            if (!_tokenHandler.VerifyPasswordHash(dto.Password, user.Password))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                {
                    user.Status = UserStatus.Blocked;
                    user.BlockedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    throw new UserAccessException("Hesabınız təhlükəsizlik məqsədilə bloklanıb. " +
                                                  "Çoxsaylı uğursuz giriş cəhdləri aşkar edilib. " +
                                                  "Zəhmət olmasa dəstək xidməti ilə əlaqə saxlayın.");
                }

                await _context.SaveChangesAsync();
                throw new LoginFailedException($"Şifrə yanlışdır. {5 - user.FailedLoginAttempts}");
            }

            user.FailedLoginAttempts = 0;
            await _context.SaveChangesAsync();

            return await GenerateTokenResponse(user);
        }


        public async Task<TokenResponseDto> LoginWithRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new RefreshTokenExpiredException();

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken && x.RefreshTokenExpireDate > DateTime.UtcNow)
                ?? throw new LoginFailedException();

            return await GenerateTokenResponse(user);
        }

        public async Task ResetPasswordAsync(string input)
        {
            var normalizedInput = input.ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email.ToLower() == normalizedInput ||
                    x.UserName.ToLower() == normalizedInput)
                ?? throw new UserNotFoundException();

            var token = _tokenHandler.CreatePasswordResetToken(user);

            await _context.PasswordTokens.AddAsync(new PasswordToken
            {
                Token = token,
                UserId = user.Id,
                ExpireTime = DateTime.UtcNow.AddHours(1)
            });

            await _context.SaveChangesAsync();
        }

        public async Task ConfirmPasswordResetAsync(PasswordResetDto dto)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(dto.Token);
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email.ToLower())
                ?? throw new UserNotFoundException();

            var passwordToken = await _context.PasswordTokens
                .FirstOrDefaultAsync(pt => pt.Token == dto.Token && pt.UserId == user.Id && pt.ExpireTime > DateTime.UtcNow)
                ?? throw new BadRequestException("Invalid or expired token");

            user.Password = _tokenHandler.GeneratePasswordHash(dto.NewPassword);
            _context.PasswordTokens.Remove(passwordToken);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePasswordAsync(PasswordChangeDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == _currentUser.UserGuid)
                ?? throw new NotFoundException<User>();

            if (!_tokenHandler.VerifyPasswordHash(dto.CurrentPassword, user.Password))
                throw new OldPasswordWrongException();

            user.Password = _tokenHandler.GeneratePasswordHash(dto.NewPassword);
            await _context.SaveChangesAsync();
        }

        private async Task<TokenResponseDto> GenerateTokenResponse(User user)
        {
            var accessToken = _tokenHandler.CreateToken(user, 60);
            var refreshToken = _tokenHandler.GenerateRefreshToken(accessToken, 1440);

            user.RefreshToken = refreshToken.Token;
            user.RefreshTokenExpireDate = refreshToken.Expires;
            await _context.SaveChangesAsync();

            var response = new TokenResponseDto
            {
                UserId = user.Id.ToString(),
                FullName = $"{user.FirstName} {user.LastName}",
                UserStatusId = (byte)user.UserRole,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Expires = refreshToken.Expires,
            };

            return response;
        }

        

        public async Task LogOutAsync(LogoutDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.AccessToken))
                throw new ArgumentException("AccessToken is required");

            var userIdFromToken = _tokenHandler.GetUserIdFromToken(dto.AccessToken);

            if (userIdFromToken == Guid.Empty)
                throw new UnauthorizedAccessException("Token does not contain a valid user ID");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userIdFromToken)
                ?? throw new NotFoundException<User>();

            user.RefreshToken = null;
            user.RefreshTokenExpireDate = null;

            var isAlreadyBlacklisted = await _context.BlacklistedTokens
                .AnyAsync(x => x.Token == dto.AccessToken);

            if (!isAlreadyBlacklisted)
            {
                await _context.BlacklistedTokens.AddAsync(new BlacklistedToken
                {
                    Token = dto.AccessToken,
                    ExpireTime = DateTime.UtcNow,
                    UserId = user.Id,
                    Reason = "User logout"
                });
            }

            await _context.SaveChangesAsync();
        }

        
    }
}
