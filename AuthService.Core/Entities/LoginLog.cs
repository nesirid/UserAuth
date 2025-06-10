using AuthService.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Entities
{
    public class LoginLog : BaseEntity
    {
        public DateTime LoginDate { get; set; }

        public Guid UserId { get; set; }

        public bool IsSucceed { get; set; }

        public string? IP { get; set; }

        public virtual User? User { get; set; }
    }
}
