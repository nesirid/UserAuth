using AuthService.Core.Base;
using SharedLibrary.Enums;

namespace AuthService.Core.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;

        public string JobPosition { get; set; }

        public string Password { get; set; } = null!;

        public string MainPhoneNumber { get; set; }
        public string Email { get; set; } = null!;

        public UserStatus Status { get; set; } = UserStatus.Active;
        public UserRole UserRole { get; set; }

        public DateTime? RefreshTokenExpireDate { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime? LockDownDate { get; set; }

        public string? Image { get; set; }

        public virtual ICollection<LoginLog> LoginLogs { get; set; } = new List<LoginLog>();
        public string? RefreshToken { get; set; }
        public PasswordToken? PasswordToken { get; set; }
    }
}
