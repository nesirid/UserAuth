using AuthService.Core.Base;

namespace AuthService.Core.Entities
{
    public class PasswordToken : BaseEntity
    {
        public string Token { get; set; }

        public DateTime ExpireTime { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
