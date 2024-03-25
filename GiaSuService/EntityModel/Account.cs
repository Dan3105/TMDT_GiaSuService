using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Account
{
    public int Id { get; set; }

    public string Fullname { get; set; } = null!;

    public DateOnly Birth { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string Identitycard { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public bool Lockenable { get; set; }

    public string Avatar { get; set; } = null!;

    public int Roleid { get; set; }

    public string Addressdetail { get; set; } = null!;

    public int Districtid { get; set; }

    public string Frontidentitycard { get; set; } = null!;

    public string Backidentitycard { get; set; } = null!;

    public DateOnly Createdate { get; set; }

    public virtual ICollection<Configpricehistory> Configpricehistories { get; set; } = new List<Configpricehistory>();

    public virtual District District { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Transaction> TransactionAccountpays { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionAccountwrites { get; set; } = new List<Transaction>();

    public virtual ICollection<Tutorrequestform> Tutorrequestforms { get; set; } = new List<Tutorrequestform>();

    public virtual Tutorprofile? Tutorprofile { get; set; }
}
