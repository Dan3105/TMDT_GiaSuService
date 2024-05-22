namespace GiaSuService.Models.CustomerViewModel
{
    public class CustomerTutorRequestViewModel
    {
        public int RequestId { get; set; }
        public int Students { get; set; }

        public string? AdditionalDetail { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string AddressDetail { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string StatusVietnamese {  get; set; } = string.Empty;
    }
}
