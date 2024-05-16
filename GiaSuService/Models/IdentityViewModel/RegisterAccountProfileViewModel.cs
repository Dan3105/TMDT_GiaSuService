using GiaSuService.Configs;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.IdentityViewModel
{
    public class RegisterAccountProfileViewModel
    {
        public string Avatar { get; set; } = AppConfig.DEFAULT_AVATAR_URL;

        [Required(ErrorMessage = "Vui lòng không để trống họ và tên.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Họ và tên chứa 5-100 ký tự.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Họ và tên chỉ được chứa ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng không để trống email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng không để trống số điện thoại.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại chỉ bao gồm 10 chữ số.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng không để trống ngày sinh.")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
        public string Gender { get; set; } = string.Empty;

        
        [Required(ErrorMessage = "Vui lòng không để trống CMND/CCCD.")]
        [RegularExpression(@"^\d{9}$|^\d{12}$", ErrorMessage = "CMND chỉ gồm 9 chữ số hoặc CCCD chỉ gồm 12 chữ số.")]
        public string IdentityCard { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng tải ảnh mặt trước CMND/CCCD.")]
        public string FrontIdentityCard { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng tải ảnh mặt sau CMND/CCCD.")]
        public string BackIdentityCard { get; set; } = string.Empty;


        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu mới gồm 6 - 20 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu.")]
        [Compare(nameof(Password), ErrorMessage = "Nhập lại mật khẩu không trùng với mật khẩu.")]
        public string ConfirmPassword { get; set; } = string.Empty;


        [Required(ErrorMessage = "Vui lòng không để trống số nhà, tên đường.")]
        [StringLength(255, ErrorMessage = "Số nhà, tên đường chứa không quá 255 ký tự.")]
        public string AddressName { get; set; } = string.Empty;

        [Required]
        public int SelectedDistrictId { get; set; }

        public RegisterAccountProfileViewModel() { }
    }
}
