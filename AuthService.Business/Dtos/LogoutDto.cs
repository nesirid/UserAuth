﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Dtos
{
    public class LogoutDto
    {
        public string? AccessToken { get; set; }

        public DateTime? ExpireTime { get; set; }
    }
}
