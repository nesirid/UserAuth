using AuthService.Business.Dtos;
using AuthService.Business.Services.AuthService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Helpers;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            await _authService.RegisterAsync(dto);
            return Ok();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            return Ok(await _authService.LoginAsync(dto));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LoginWithRefreshToken(string refreshToken)
        {
            return Ok(await _authService.LoginWithRefreshTokenAsync(refreshToken));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RequestPasswordReset(string email)
        {
            await _authService.ResetPasswordAsync(email);
            return Ok();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("[action]")]
        public async Task<IActionResult> UnblockUser([FromBody] Guid userId)
        {
            await _authService.UnBlockUserAsync(userId);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ConfirmPasswordReset(PasswordResetDto resetDto)
        {
            await _authService.ConfirmPasswordResetAsync(resetDto);
            return Ok();
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdatePassword(PasswordChangeDto dto)
        {
            await _authService.UpdatePasswordAsync(dto);
            return Ok();
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> CheckUserExist([FromQuery] UserCheckDto dto)
        {
            await _authService.CheckUserExistAsync(dto);
            return Ok();
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> LogOut([FromBody] LogoutDto Dto)
        {
            await _authService.LogOutAsync(Dto);
            return Ok();
        }

    }
}
