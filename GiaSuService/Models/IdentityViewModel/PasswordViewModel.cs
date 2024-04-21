using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.IdentityViewModel
{
    public class PasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu mới gồm 6 - 20 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu mới.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu mới gồm 6 - 20 ký tự")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public PasswordViewModel() { }
    }
}
