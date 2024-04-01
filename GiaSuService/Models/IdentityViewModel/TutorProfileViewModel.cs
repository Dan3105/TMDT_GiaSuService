using GiaSuService.Configs;
using GiaSuService.EntityModel;

namespace GiaSuService.Models.IdentityViewModel
{
    public class TutorProfileViewModel
    {
        #region Account Profile
        public string Fullname { get; set; } = string.Empty;
        public DateOnly? Birth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Identitycard { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public bool Lockenable { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public string Frontidentitycard { get; set; } = string.Empty;
        public string Backidentitycard { get; set; } = string.Empty;
        public DateOnly? Createdate { get; set; }
        public string Address { get; set; } = string.Empty;
        #endregion


        #region Tutor Profile
        public int TutorId { get; set; }
        public string College { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string? Additionalinfo { get; set; }
        public short Academicyearfrom { get; set; }
        public short Academicyearto { get; set; }
        public string? TypeTutor { get; set; }
        public string? Formstatus { get; set; }
        #endregion

    }
}
