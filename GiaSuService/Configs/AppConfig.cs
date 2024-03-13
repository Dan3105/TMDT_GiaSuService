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

        public enum PaymentMethod
        {
            OFFLINE,
            ONLINE
        }

        public enum ClassTutorQueue
        {
            PENDING,
            APPROVAL,
            DENY,
            REVIEWING,
            HANDED
        }

        public const string CLAIM_TYPE_AVATAR = "Avatar";

        public const string CLAIM_USER = "User";
        public const string AUTHSCHEME = "MyAuthScheme";

        public const string ADMINROLENAME = "admin";
        public const string ADMINPOLICY = "admin_policy";

        public const string EMPLOYEEROLENAME = "employee";
        public const string EMPLOYEEPOLICY = "employee_policy";

        public const string TUTORROLENAME = "tutor";
        public const string TUTORPOLICY = "tutor_policy";
    }
}
