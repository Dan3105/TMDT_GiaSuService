namespace GiaSuService.Models.UtilityViewModel
{
    public class TutorRequestStatisticsViewModel
    {
        public int TotalExpired { get; set; } = 0;
        public int TotalPending { get; set; } = 0;
        public int TotalApproval { get; set; } = 0;
        public int TotalDeny { get; set; } = 0;
        public int TotalHandover { get; set; } = 0;
        public int TotalCancel { get; set; } = 0;
    }

    public class TutorRequestStatisticCreateViewModel
    {
        public string jsonTutorRequestCreate { get; set; } = string.Empty;
        public string jsonTutorRequestStatus { get; set; } = string.Empty;
        public string jsonTutorRequestSubject { get; set; } = string.Empty;
    }
}
