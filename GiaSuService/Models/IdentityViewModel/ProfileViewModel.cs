using GiaSuService.Configs;
using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.IdentityViewModel
{
    public class ProfileViewModel
    {
        public int ProfileId { get; set; }  // employee_id or customer_id
        public int IdentityId { get; set; }
        public int AccountId { get; set; }

        public required string Avatar { get; set; } = AppConfig.DEFAULT_AVATAR_URL;

        [Required(ErrorMessage = "Vui lòng không để trống họ và tên.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Họ và tên chứa 5-100 ký tự.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Họ và tên chỉ được chứa ký tự.")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống số điện thoại.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại chỉ bao gồm 10 chữ số.")]
        
        public required string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống ngày sinh.")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
        public required string Gender { get; set; }


        [Required(ErrorMessage = "Vui lòng không để trống CMND/CCCD.")]
        [RegularExpression(@"^\d{9}$|^\d{12}$", ErrorMessage = "CMND chỉ gồm 9 chữ số hoặc CCCD chỉ gồm 12 chữ số.")]
        public required string IdentityCard { get; set; }
        public required string FrontIdentityCard { get; set; } = AppConfig.DEFAULT_FRONT_IDENTITY_CARD_URL;
        public required string BackIdentityCard { get; set; } = AppConfig.DEFAULT_BACK_IDENTITY_CARD_URL;

        
        public bool LockStatus { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống số nhà, tên đường.")]
        [StringLength(255, ErrorMessage = "Số nhà, tên đường chứa không quá 255 ký tự.")]
        public required string AddressDetail { get; set; }
        public int SelectedDistrictId { get; set; }
        public int SelectedProvinceId { get; set; }

        public ProfileViewModel() { }
    }
}
