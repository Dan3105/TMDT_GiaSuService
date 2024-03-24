using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.IdentityViewModel
{
    public class RegisterAccountProfileViewModel
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Full name must be between 3 and 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your birth date.")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your phone number.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid phone number. Please enter 10 digits.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your password.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 8 and 20 characters.")]
        public string Password { get; set; } = string.Empty;

        [Compare(nameof(Password), ErrorMessage = "Passwords must match.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter your identity card number.")]
        public string IdentityCard { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select your gender.")]
        public string Gender { get; set; } = "Nam";

        [StringLength(255, ErrorMessage = "Image Invalid.")]
        public string LogoAccount { get; set; } = "https://micpa.org/images/site/enews-images/cat8.jpg?sfvrsn=48f27c5e_2";

        [StringLength(255, ErrorMessage = "Image Invalid.")]
        public string FrontIdentityCard { get; set; } = "https://micpa.org/images/site/enews-images/cat8.jpg?sfvrsn=48f27c5e_2";

        [StringLength(255, ErrorMessage = "Image Invalid.")]
        public string BackIdentityCard { get; set; } = "https://micpa.org/images/site/enews-images/cat8.jpg?sfvrsn=48f27c5e_2";

        [StringLength(255, ErrorMessage = "Address must be filled")]
        public string AddressName { get; set; } = string.Empty;

        [Required]
        public int SelectedDistrictId { get; set; }

    }
}
