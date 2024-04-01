using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.IdentityViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string? LoginName { get; set; }

        [Required]
        public string? Password { get; set; }

    }
}
