using NpgsqlTypes;

namespace GiaSuService.Configs
{
    public class AppConfig
    {
        public enum TutorRequestStatus
        {
            PENDING,
            APPROVAL,
            DENIED,
            HANDED,
            EXPIRED
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

        public enum QueueStatus
        {
            PENDING,
            APPROVAL,
            DENY,
            REVEWING,
            HANDED
        }

        public enum TransactionType
        {
            DEPOSIT,
            PAYROLL,
            TUTION
        }

        public enum TypeTutor
        {
            TEACHER,
            STUDENT
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

        public const string CUSTOMERROLENAME = "customer";
        public const string CUSTOMERPOLICY = "customer_policy";

        public const string MESSAGE_SUCCESS = "message_success";
        public const string MESSAGE_FAIL = "message_failed";
    }
}
