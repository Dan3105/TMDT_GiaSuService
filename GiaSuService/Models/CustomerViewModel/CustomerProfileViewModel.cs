using System.ComponentModel.DataAnnotations;

namespace GiaSuService.Models.CustomerViewModel
{
    public class CustomerProfileViewModel
    {
        public int CustomerId { get; set; }
        public required string FullName { get; set; }
        public DateOnly BirthDate { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        [Required(ErrorMessage = "Please enter your identity card number.")]
        public required string IdentityCard { get; set; }
        public required string FrontIdentiyCard { get; set; } = "https://micpa.org/images/site/enews-images/cat8.jpg?sfvrsn=48f27c5e_2";
        public required string BackIdentityCard { get; set; } = "https://micpa.org/images/site/enews-images/cat8.jpg?sfvrsn=48f27c5e_2";
        public required string Gender { get; set; }
        public required string Avatar { get; set; } = "https://micpa.org/images/site/enews-images/cat8.jpg?sfvrsn=48f27c5e_2";
        public bool LockStatus { get; set; }
        public required string AddressDetail { get; set; }

        public CustomerProfileViewModel() { }
    }
}
