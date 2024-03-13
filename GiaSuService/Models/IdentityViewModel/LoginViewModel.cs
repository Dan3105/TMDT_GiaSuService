using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.IdentityViewModel
{
    public class LoginViewModel
    {
        [EmailAddress]
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

    }
}
