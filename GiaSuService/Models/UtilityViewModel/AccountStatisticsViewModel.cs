using GiaSuService.Configs;

namespace GiaSuService.Models.UtilityViewModel
{
    public struct StatisticViewModel
    {
        public List<string> labels { get; set; }
        public List<int> data { get; set; }
        public int total
        {
            get
            {
                int count = 0;
                foreach (var d in data)
                {
                    count += d;
                }
                return count;
            }
            set
            {
                total = value;
            }
        }
    }


    public class AccountStatisticsViewModel
    {
        
        public string jsonCustomerStatisc { set; get; } = string.Empty;
        public string jsonTutorStatisc { set; get; } = string.Empty;
        public string jsonEmployeeStatisc { set; get; } = string.Empty;

        public string jsonTutorStatusStatisc { get; set; } = string.Empty;
        public string jsonTutorActiveStatisc { get; set; } = string.Empty;
    }

    public class AccountRegisterStatisticsViewModel
    {
        public string jsonRegisterStatisc { set; get; } = string.Empty;
    }
}
