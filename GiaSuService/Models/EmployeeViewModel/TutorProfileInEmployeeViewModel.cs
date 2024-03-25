using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Models.EmployeeViewModel
{
    public class TutorProfileInEmployeeViewModel
    {
        #region Account Profile
        public string Fullname { get; set; } = null!;
        public DateOnly Birth { get; set; }
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Identitycard { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public bool Lockenable { get; set; }
        public string Avatar { get; set; } = null!;
        public string Frontidentitycard { get; set; } = null!;
        public string Backidentitycard { get; set; } = null!;
        public DateOnly? Createdate { get; set; }
        public string Address { get; set; } = string.Empty;
        #endregion


        #region Tutor Profile
        public int TutorId { get; set; }
        public string College { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string? Additionalinfo { get; set; }
        public short Academicyearfrom { get; set; }
        public short Academicyearto { get; set; }
        public string? Currentstatus { get; set; }
        public AppConfig.RegisterStatus Formstatus { get; set; } = AppConfig.RegisterStatus.PENDING;
        #endregion

    }
}
