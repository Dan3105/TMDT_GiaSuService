using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Models.IdentityViewModel
{
    public class TutorProfileViewModel
    {
        #region Account Profile
        public int AccountId { get; set; }
        public int IdentityId { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public DateOnly? Birth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string IdentityCard { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public bool Lockenable { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public string FrontIdentityCard { get; set; } = string.Empty;
        public string BackIdentityCard { get; set; } = string.Empty;
        public DateOnly? Createdate { get; set; }
        public bool LockStatus { get; set; }
        public required string AddressDetail { get; set; }
        public int SelectedDistrictId { get; set; }
        public int SelectedProvinceId { get; set; }
        #endregion


        #region Tutor Profile
        public int TutorId { get; set; }
        public string College { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Additionalinfo { get; set; }
        public short Academicyearfrom { get; set; }
        public short Academicyearto { get; set; }
        public bool TypeTutor { get; set; }
        public string? Formstatus { get; set; }
        #endregion

    }
}
