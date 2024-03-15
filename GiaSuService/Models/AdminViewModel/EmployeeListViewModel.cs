namespace GiaSuService.Models.AdminViewModel
{
    public class EmployeeListViewModel
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email {  get; set; }
        public bool LockStatus { get; set; }
        public required string ImageUrl { get; set; }
    }
}
