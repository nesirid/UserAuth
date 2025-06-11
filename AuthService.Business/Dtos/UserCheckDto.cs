using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Dtos
{
    public class UserCheckDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UserCheckDtoValidator : AbstractValidator<UserCheckDto>
    {
        public UserCheckDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email boş ola bilməz")
                .EmailAddress()
                .WithMessage("Düzgün email formatı deyil");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Telefon nömrəsi boş ola bilməz");
        }
    }
}
