using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models
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
