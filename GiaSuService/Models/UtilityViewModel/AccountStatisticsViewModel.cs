namespace GiaSuService.Models.UtilityViewModel
{
    public class AccountStatisticsViewModel
    {
        public int TotalCustomer {  get; set; }
        public int TotalTutor { get; set; }
        public int TotalEmployee { get; set; }

        public Dictionary<int, string> ListRole { get; set; } = new Dictionary<int, string>();
    }
}
