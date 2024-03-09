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

    public string Logoaccount { get; set; } = null!;

    public int Addressid { get; set; }

    public int Roleid { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Tutorprofile> Tutorprofiles { get; set; } = new List<Tutorprofile>();
}
