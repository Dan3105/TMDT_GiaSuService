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

    public bool Lockenable { get; set; } = false;

    public string Logoaccount { get; set; } = null!;

    public int Roleid { get; set; }

    public int Addressid { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Tutorprofile> Tutorprofiles { get; set; } = new List<Tutorprofile>();
}
