using AuthService.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Entities
{
    public class BlacklistedToken : BaseEntity
    {
        public string Token { get; set; } = null!;
        public Guid UserId { get; set; }
        public DateTime? ExpireTime { get; set; }
        public string Reason { get; set; } = "Unknown";
    }
}
