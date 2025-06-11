using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.HelperServices.Current.Interface
{
    public interface ICurrentUser
    {
        public string? UserId { get; }
        public Guid? UserGuid { get; }
        public string? UserFullName { get; }
        public string? BaseUrl { get; }
        public byte UserRole { get;}
    }
}
