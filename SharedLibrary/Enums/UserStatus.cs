using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Enums
{
    public enum UserStatus : byte
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3,
        Deleted = 4,
        Blocked = 5,
        Archived = 6
    }
}
