using GiaSuService.Models.IdentityViewModel;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.AdminViewModel
{
    public class RegisterEmployeeViewModel
    {
        public RegisterFormViewModel? RegisterForm { get; set; }
        public List<ProvinceViewModel> ProvinceList { get; set;} = new List<ProvinceViewModel>();

        public RegisterEmployeeViewModel() { }
    }

    public class RegisterFormViewModel
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Full name must be between 3 and 100 characters.")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Please enter your birth date.")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Please enter your phone number.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid phone number. Please enter 10 digits.")]
        public required string Phone { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 8 and 20 characters.")]
        public required string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Passwords must match.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter your identity card number.")]
        public required string IdentityCard { get; set; }

        [Required(ErrorMessage = "Please select your gender.")]
        public required string Gender { get; set; }

        [StringLength(255, ErrorMessage = "Image Invalid characters.")]
        public required string LogoAccount { get; set; } = "https://micpa.org/images/site/enews-images/cat8.jpg?sfvrsn=48f27c5e_2";

        public AddressViewModel? AddressVM { get; set; }
    }
}
