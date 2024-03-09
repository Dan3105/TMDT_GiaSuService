namespace GiaSuService.Configs
{
    public class AppConfig
    {
        public enum ClassStatus
        {
            PENDING, 
            APPROVAL,
            DENIED, 
            HANDED, 
            OUTDATED
        }

        public enum RegisterStatus
        {
            PENDING,
            APPROVAL,
            DENY,
        }
    }
}
