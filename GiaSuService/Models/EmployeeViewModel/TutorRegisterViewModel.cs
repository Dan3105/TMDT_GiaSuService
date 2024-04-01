namespace GiaSuService.Models.EmployeeViewModel
{
    public class TutorRegisterViewModel
    {
        public int Id { get; set; }
        public bool IsValid { get; set; }
        public string FullName { get; set; }  = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string College { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public string StatusQuery { get; set; } = string.Empty;
        public DateOnly CreateDate {  get; set; }
    }
}
