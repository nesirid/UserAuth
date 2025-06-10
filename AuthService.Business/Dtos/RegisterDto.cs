using FluentValidation;
using SharedLibrary.Helpers;

namespace AuthService.Business.Dtos
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string JobPosition { get; set; }
        public string MainPhoneNumber { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }

    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            // Ad validasiyası
            RuleFor(x => x.FirstName)
                .NotNull().WithMessage("Ad boş ola bilməz")
                .NotEmpty().WithMessage("Ad boş ola bilməz")
                .Length(1, 50).WithMessage("Ad 1-50 simvol aralığında olmalıdır");

            // Soyad validasiyası
            RuleFor(x => x.LastName)
                .NotNull().WithMessage("Soyad boş ola bilməz")
                .NotEmpty().WithMessage("Soyad boş ola bilməz")
                .Length(1, 50).WithMessage("Soyad 1-50 simvol aralığında olmalıdır");

            // İstifadəçi adı validasiyası
            RuleFor(x => x.UserName)
                .NotNull().WithMessage("İstifadəçi adı boş ola bilməz")
                .NotEmpty().WithMessage("İstifadəçi adı boş ola bilməz")
                .Length(3, 30).WithMessage("İstifadəçi adı 3-30 simvol aralığında olmalıdır");

            // İş yeri validasiyası
            RuleFor(x => x.JobPosition)
                .NotNull().WithMessage("Vəzifə boş ola bilməz")
                .NotEmpty().WithMessage("Vəzifə boş ola bilməz")
                .Length(2, 100).WithMessage("Vəzifə 2-100 simvol aralığında olmalıdır");

            // Telefon nömrəsi validasiyası
            RuleFor(x => x.MainPhoneNumber)
                .NotNull().WithMessage("Telefon nömrəsi boş ola bilməz")
                .NotEmpty().WithMessage("Telefon nömrəsi boş ola bilməz")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Yanlış telefon nömrəsi formatı");

            // Email validasiyası
            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email boş ola bilməz")
                .NotEmpty().WithMessage("Email boş ola bilməz")
                .EmailAddress().WithMessage("Yanlış email formatı");

            // Şifrə validasiyası
            RuleFor(x => x.Password)
                .NotNull().WithMessage("Şifrə boş ola bilməz")
                .NotEmpty().WithMessage("Şifrə boş ola bilməz")
                .MinimumLength(8).WithMessage("Şifrə ən az 8 simvol olmalıdır")
                .Matches(@"[A-Z]").WithMessage("Şifrədə ən az bir böyük hərf olmalıdır")
                .Matches(@"[a-z]").WithMessage("Şifrədə ən az bir kiçik hərf olmalıdır")
                .Matches(@"[0-9]").WithMessage("Şifrədə ən az bir rəqəm olmalıdır")
                .Matches(@"[\W_]").WithMessage("Şifrədə ən az bir xüsusi simvol olmalıdır")
                .Length(8, 100).WithMessage("Şifrə 8-100 simvol aralığında olmalıdır");

            // Şifrə təsdiqi validasiyası
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Şifrələr uyğun gəlmir");
        }
    }
}
