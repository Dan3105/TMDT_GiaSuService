﻿using GiaSuService.EntityModel;
using NpgsqlTypes;
using System.Globalization;

namespace GiaSuService.Configs
{
    public class AppConfig
    {
        public const string connection_string = "TutorConnection";

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
            HANDOVER,
            REFUND,
            CANCEL,
        }

        public const string transaction_status = "Transaction";
        public enum TransactionStatus
        {
            PENDING,
            PAID,
            CANCEL,
        }

        public enum TransactionFilterStatus
        {
            ALL,
            PAID,
            UNPAID,
        }

        public enum TransactionFilterType
        {
            ALL,
            PAID,
            REFUND,
        }

        public enum UploadFileType
        {
            AVATAR,
            FRONT_IDENTITY_CARD,
            BACK_IDENTITY_CARD,
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

        public const string PROFILE_POLICY = "profile_policy";

        public const string MESSAGE_SUCCESS = "message_success";
        public const string MESSAGE_FAIL = "message_failed";

        public const string TUTOR_SELECTED_COOKIE = "tutors_cookie";

        public const string DEFAULT_AVATAR_URL = "https://icons.veryicon.com/png/o/miscellaneous/rookie-official-icon-gallery/225-default-avatar.png";
        public const string DEFAULT_FRONT_IDENTITY_CARD_URL = "https://micpa.org/images/site/enews-images/cat8.jpg?sfvrsn=48f27c5e_2";
        public const string DEFAULT_BACK_IDENTITY_CARD_URL = "https://micpa.org/images/site/enews-images/cat8.jpg?sfvrsn=48f27c5e_2";

        public const int ROWS_ACCOUNT_LIST = 10;
        public static string ContextForApplyTutor(string empName, string empphone) 
            => $"Gia sư đến đóng phí nhận lớp tại trung tâm";

        public static string ContextForRefundTransaction(string tutorName, string empphone, string empName, decimal price)
        {
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo("vi-VN");

            string vndString = price.ToString("c", cultureInfo);
            return $"Hoàn trả {vndString} cho gia sư {tutorName}, được thực hiện bởi {empName}, có thể liên lạc thông tin của nhân viên qua số điện thoại {empphone}";
        }

        public const bool DEPOSIT_TYPE = true;
    }
}
