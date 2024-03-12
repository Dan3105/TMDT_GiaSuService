namespace GiaSuService.Models
{
    [Serializable]
    public class UserAuthenticationModel
    {
        public string? FullName { get; set; }
        public int? IdUser { get; set; }
        public string? Role {  get; set; }
    }
}
