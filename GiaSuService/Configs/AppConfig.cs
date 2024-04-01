using NpgsqlTypes;

namespace GiaSuService.Configs
{
    public class AppConfig
    {
        public const string register_status = "Register";
        public enum RegisterStatus
        {
            PENDING,
            APPROVAL,
            DENY,
            UPDATE,
        }

        public const string form_status = "Form";
        public enum FormStatus
        {
            PENDING,
            APPROVAL,
            DENY,
            HANDOVER,
            CANCEL,
        }

        public const string queue_status = "Queue";
        public enum QueueStatus
        {
            PENDING,
            APPROVAL,
            DENY,
            HANDOVER
        }

        //public enum PaymentMethod
        //{
        //    OFFLINE,
        //    ONLINE
        //}


        //public enum TransactionhistoryType
        //{
        //    DEPOSIT,
        //    PAYROLL,
        //    TUTION
        //}

        //public enum TypeTutor
        //{
        //    TEACHER,
        //    STUDENT
        //}

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

        public const string TUTOR_SELECTED_COOKIE = "tutors_cookie";

        public const int ROWS_ACCOUNT_LIST = 10;
    }
}
