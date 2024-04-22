namespace GiaSuService.Models.UtilityViewModel
{
    public class TutorRequestStatisticsViewModel
    {
        public int TotalCreated { get; set; }
        public int TotalPending { get; set; } = 0;
        public int TotalApproval { get; set; } = 0;
        public int TotalDeny { get; set; } = 0;
        public int TotalHandover { get; set; } = 0;
        public int TotalCancel { get; set; } = 0;

        public Dictionary<string, int> TopKPopular { get; set; } = new Dictionary<string, int>();
    }
}
